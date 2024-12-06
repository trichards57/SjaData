// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using HealthChecks.ApplicationStatus.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Validation.AspNetCore;
using Quartz;
using SjaData.Server.Authorization;
using SjaData.Server.Data;
using SjaData.Server.Services;
using SjaData.Server.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.EnableEnrichment();

builder.Logging.AddApplicationInsights(
    configureTelemetryConfiguration: (config) => config.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"],
    configureApplicationInsightsLoggerOptions: (options) => { });

builder.Services.AddApplicationMetadata(x =>
{
    x.ApplicationName = "SJA in Numbers";
    x.EnvironmentName = builder.Environment.EnvironmentName;
    x.BuildVersion = typeof(Program).Assembly.GetName().Version?.ToString();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SjaData.Server.xml"));
});

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

builder.Services.AddScoped<IDeploymentService, DeploymentService>();
builder.Services.AddScoped<IDistrictService, DistrictService>();
builder.Services.AddScoped<IHoursService, HoursService>();
builder.Services.AddScoped<IHubService, HubService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IAuthorizationHandler, RequireApprovalHandler>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
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

var app = builder.Build();

app.UseDefaultFiles();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

await app.RunAsync();
