// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using OpenIddict.Client;
using System.Globalization;
using System.Net.Http.Json;
using System.Reflection;

namespace VorUploader;

/// <summary>
/// Main program class.
/// </summary>
internal static class Program
{
    private const string InboxId = "AAMkAGU0NTA2ODczLTYyZWEtNGZhNS04MDQ0LTIwMDFlMDQzN2M0ZQAuAAAAAAB20NTeTHozS4sqw68MW0ExAQBWg0hGD4wFSaUCnLIUVcwOAAAA-DKkAAA=";
    private const string VorFolderId = "AAMkAGU0NTA2ODczLTYyZWEtNGZhNS04MDQ0LTIwMDFlMDQzN2M0ZQAuAAAAAAB20NTeTHozS4sqw68MW0ExAQD7P9wa3zxsSruC4g1eWMZTAATImwK9AAA=";
    private const string TokenReplyString = "http://localhost/";

    private static async Task CancelToken(IServiceProvider provider, string token)
    {
        var service = provider.GetRequiredService<OpenIddictClientService>();

        await service.RevokeTokenAsync(new() { Token = token });
    }

    private static async Task<string> GetTokenAsync(IServiceProvider provider)
    {
        var service = provider.GetRequiredService<OpenIddictClientService>();

        var result = await service.AuthenticateWithClientCredentialsAsync(new());
        return result.AccessToken;
    }

    private static async Task Main()
    {
        var builder = new ConfigurationBuilder();

        builder.SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory) ?? Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<VorIncident>(true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        var baseUri = new Uri(configuration["BaseUri"] ?? throw new InvalidOperationException("BaseUri not set in configuration."));

        var services = new ServiceCollection();
        services.AddOpenIddict()
            .AddClient(o =>
            {
                o.AllowClientCredentialsFlow();
                o.DisableTokenStorage();
                o.UseSystemNetHttp().SetProductInformation(typeof(Program).Assembly);
                o.AddRegistration(new OpenIddictClientRegistration
                {
                    Issuer = baseUri,
                    ClientId = configuration["OpenIdWorkerSettings:VorUploaderClientId"],
                    ClientSecret = configuration["OpenIdWorkerSettings:VorUploaderClientSecret"],
                });
            });

        await using var provider = services.BuildServiceProvider();

        // Get AO Dashboard Token
        var token = await GetTokenAsync(provider);

        using var httpClient = new HttpClient()
        {
            BaseAddress = baseUri,
        };

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var vorUris = new Uri("/api/vor", UriKind.Relative);

        var files = Directory.EnumerateFiles(Environment.CurrentDirectory, "*.xls").ToList();

        foreach (var file in files)
        {
            var fileDate = DateOnly.Parse(Path.GetFileNameWithoutExtension(file).Split(" ")[0], CultureInfo.InvariantCulture);

            var items = FileParser.ParseFile(file, fileDate);

            var count = 0;
            HttpResponseMessage result;

            do
            {
                if (count > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }

                result = await httpClient.PostAsJsonAsync(vorUris, items);
                count++;
            }
            while (count < 3 && !result.IsSuccessStatusCode);

            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine($"Received Error : {result.StatusCode}");
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Uploaded"));
                File.Move(file, Path.Combine(Environment.CurrentDirectory, "Uploaded", Path.GetFileName(file)), true);
            }
        }

        // Get a Microsoft Graph token
        var scopes = new[] { "User.Read", "Mail.ReadWrite" };
        var graphClientId = configuration["Authentication:Microsoft:ClientId"];
        var tenantId = "91d037fb-4714-4fe8-b084-68c083b8193f";

        var options = new InteractiveBrowserCredentialOptions()
        {
            TenantId = tenantId,
            ClientId = graphClientId,
            RedirectUri = new Uri(TokenReplyString),
            LoginHint = "tony.richards1@sja.org.uk",
        };

        var interactiveCredential = new InteractiveBrowserCredential(options);

        using var graphClient = new GraphServiceClient(interactiveCredential, scopes);

        var emailResponse = await graphClient.Me.MailFolders[InboxId]
            .Messages
            .GetAsync(r =>
            {
                r.QueryParameters.Top = 100;
            });

        var emails = new List<Message>();

        if (emailResponse != null)
        {
            var pageIterator = PageIterator<Message, MessageCollectionResponse>.CreatePageIterator(graphClient, emailResponse, e =>
            {
                if (e.Subject?.Equals("Daily VOR Report", StringComparison.OrdinalIgnoreCase) == true)
                {
                    emails.Add(e);
                }

                return true;
            });

            await pageIterator.IterateAsync();
        }

        foreach (var e in emails)
        {
            var attachments = await graphClient.Me.Messages[e.Id].Attachments.GetAsync();
            var attachment = attachments?.Value?.Find(a => a.Name?.EndsWith(".xls", StringComparison.OrdinalIgnoreCase) == true);

            if (attachment == null
                || e.ReceivedDateTime == null
                || attachment is not FileAttachment fa
                || fa.ContentBytes == null)
            {
                continue;
            }

            var fileDate = DateOnly.FromDateTime(e.ReceivedDateTime.Value.Date);
            Console.WriteLine($"Found File - Date: {e.ReceivedDateTime}");

            var tempFile = Path.GetRandomFileName();
            await File.WriteAllBytesAsync(tempFile, fa.ContentBytes);

            var items = FileParser.ParseFile(tempFile, fileDate);

            var count = 0;
            HttpResponseMessage result;

            do
            {
                if (count > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }

                result = await httpClient.PostAsJsonAsync(vorUris, items);
                count++;
            }
            while (count < 3 && !result.IsSuccessStatusCode);

            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine($"Received Error : {result.StatusCode}");
            }
            else
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Uploaded"));
                File.Move(tempFile, Path.Combine(Environment.CurrentDirectory, "Uploaded", $"{fileDate:o} VOR Report.xls"), true);
                await graphClient.Me.Messages[e.Id].Move.PostAsync(new Microsoft.Graph.Me.Messages.Item.Move.MovePostRequestBody { DestinationId = VorFolderId });
            }
        }

        await CancelToken(provider, token);
    }
}
