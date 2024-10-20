// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Asp.Versioning;
using HealthChecks.ApplicationStatus.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using Quartz;
using SjaInNumbers.Server.Authorization;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Server.Helpers;
using SjaInNumbers.Server.Services;
using SjaInNumbers.Server.Services.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

builder.Services.ConfigureApplicationCookie(o =>
{
    o.Events.OnRedirectToLogin = c =>
    {
        c.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    o.Events.OnRedirectToAccessDenied = c =>
    {
        c.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
{
    var tenantId = builder.Configuration["Authentication:Microsoft:TenantId"] ?? throw new InvalidOperationException("No Microsoft Tenant ID");
    microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"] ?? throw new InvalidOperationException("No Microsoft Client ID");
    microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"] ?? throw new InvalidOperationException("No Microsoft Client Secret");
    microsoftOptions.AuthorizationEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize";
    microsoftOptions.TokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Approved", o => o.AddRequirements(new RequireApprovalRequirement()))
    .AddPolicy("Admin", o => o.RequireRole("Admin").AddRequirements(new RequireApprovalRequirement()))
    .AddPolicy("Lead", o => o.RequireRole("Admin", "Lead").AddRequirements(new RequireApprovalRequirement()))
    .AddPolicy("Uploader", o => o.RequireClaim("VorData", "Edit"));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IHoursService, HoursService>();
builder.Services.AddScoped<IHubService, HubService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IAuthorizationHandler, RequireApprovalHandler>();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<SwaggerDefaultValues>();
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SjaInNumbers.Server.xml"));
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SjaInNumbers.Shared.xml"));
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseOpenIddict();
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

const string LocalScheme = "LocalScheme";

builder.Services.AddAuthentication(LocalScheme)
    .AddPolicyScheme(LocalScheme, "Either Authorization bearer Header or Auth Cookie", o =>
    {
        o.ForwardDefaultSelector = c =>
        {
            var authHeader = c.Request.Headers.Authorization.FirstOrDefault();
            if (authHeader?.StartsWith("Bearer ") == true)
            {
                return OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            }

            return IdentityConstants.ApplicationScheme;
        };
    });

builder.Services.AddOpenIddict()
    .AddCore(o =>
    {
        o.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>();
        o.UseQuartz();
    })
    .AddServer(o =>
    {
        o.SetTokenEndpointUris("/connect/token");
        o.SetRevocationEndpointUris("/connect/revoke");
        o.AllowClientCredentialsFlow();
        o.AddEphemeralEncryptionKey();
        o.AddEphemeralSigningKey();
        o.UseAspNetCore().EnableTokenEndpointPassthrough();
    })
    .AddValidation(o =>
    {
        o.UseLocalServer();
        o.UseAspNetCore();
    });

builder.Services.AddHostedService<OpenIdWorker>();
builder.Services.AddOptions<OpenIdWorkerSettings>().BindConfiguration("OpenIdWorkerSettings");

builder.Services.AddQuartz(o =>
{
    o.UseSimpleTypeLoader();
    o.UseInMemoryStore();
}).AddQuartzHostedService(o => o.WaitForJobsToComplete = true);

builder.Services.AddApplicationInsightsTelemetry(o =>
{
    if (builder.Environment.IsDevelopment())
    {
        o.DeveloperMode = true;
    }

    o.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

builder.Services.AddSingleton<ITelemetryInitializer, AppInsightsTelemetryInitializer>();

builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString)
    .AddApplicationStatus()
    .AddApplicationInsightsPublisher(builder.Configuration["ApplicationInsights:ConnectionString"]);

builder.Services.AddApiVersioning(o =>
{
    o.ApiVersionReader = new MediaTypeApiVersionReader();
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1);
}).AddApiExplorer();

builder.Logging.AddApplicationInsights(
    configureTelemetryConfiguration: (config) => config.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"],
    configureApplicationInsightsLoggerOptions: (options) => { });

var app = builder.Build();

app.UseRewriter(new RewriteOptions().AddRedirectToNonWwwPermanent());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI(
        options =>
        {
            foreach (var groupName in app.DescribeApiVersions().Select(d => d.GroupName))
            {
                options.SwaggerEndpoint($"/swagger/{groupName}/swagger.json", groupName);
            }
        });
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();

if (app.Environment.IsProduction())
{
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            if (ctx.File.Name.EndsWith(".svg"))
            {
                ctx.Context.Response.GetTypedHeaders()
                .CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(365),
                    Extensions = { new Microsoft.Net.Http.Headers.NameValueHeaderValue("immutable", string.Empty) },
                };
            }
        },
    });
}
else
{
    app.UseStaticFiles();
}

app.MapHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

await app.RunAsync();
