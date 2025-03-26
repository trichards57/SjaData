// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Asp.Versioning;
using FluentValidation;
using HealthChecks.ApplicationStatus.DependencyInjection;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OpenIddict.Validation.AspNetCore;
using Quartz;
using Scalar.AspNetCore;
using SjaInNumbers.Server;
using SjaInNumbers.Server.Authorization;
using SjaInNumbers.Server.Data;
using SjaInNumbers.Server.Helpers;
using SjaInNumbers.Server.Model;
using SjaInNumbers.Server.Services;
using SjaInNumbers.Server.Services.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.EnableEnrichment();

builder.Services.AddApplicationMetadata(x =>
{
    x.ApplicationName = "SJA in Numbers";
    x.EnvironmentName = builder.Environment.EnvironmentName;
    x.BuildVersion = typeof(Program).Assembly.GetName().Version?.ToString();
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

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

builder.Services.ConfigureApplicationCookie(c =>
{
    c.LoginPath = $"/api/account/login";
    c.ReturnUrlParameter = "returnUrl";
});

builder.Services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
{
    var tenantId = builder.Configuration["Authentication:Microsoft:TenantId"] ?? throw new InvalidOperationException("No Microsoft Tenant ID");
    microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"] ?? throw new InvalidOperationException("No Microsoft Client ID");
    microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"] ?? throw new InvalidOperationException("No Microsoft Client Secret");
    microsoftOptions.AuthorizationEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize";
    microsoftOptions.TokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
});

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

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Approved", o => o.AddRequirements(new RequireApprovalRequirement()))
    .AddPolicy("Admin", o => o.RequireRole("Admin").AddRequirements(new RequireApprovalRequirement()))
    .AddPolicy("Lead", o => o.RequireRole("Admin", "Lead").AddRequirements(new RequireApprovalRequirement()))
    .AddPolicy("Uploader", o => o.RequireClaim("VorData", "Edit"));

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
        o.SetAuthorizationEndpointUris("/connect/authorize");
        o.SetUserInfoEndpointUris("/connect/userinfo");
        o.AllowClientCredentialsFlow();
        o.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();
        o.AddEphemeralEncryptionKey();
        o.AddEphemeralSigningKey();
        o.UseAspNetCore().EnableTokenEndpointPassthrough().EnableAuthorizationEndpointPassthrough();
        o.RegisterScopes(Scopes.Profile, Scopes.Email, Scopes.Roles);
    })
    .AddValidation(o =>
    {
        o.UseLocalServer();
        o.UseAspNetCore();
    });

builder.Services.AddAntiforgery();

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(MapperProfile));

builder.Services.AddHsts(o =>
{
    o.Preload = true;
    o.IncludeSubDomains = true;
    o.MaxAge = TimeSpan.FromHours(1);
});

builder.Services.AddScoped<IDeploymentService, DeploymentService>();
builder.Services.AddScoped<IDistrictService, DistrictService>();
builder.Services.AddScoped<IHoursService, HoursService>();
builder.Services.AddScoped<IHubService, HubService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IAuthorizationHandler, RequireApprovalHandler>();
builder.Services.AddSingleton<ITelemetryInitializer, AppInsightsTelemetryInitializer>();

builder.Services.AddApplicationInsightsTelemetry(o =>
{
    if (builder.Environment.IsDevelopment())
    {
        o.DeveloperMode = true;
    }

    o.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

builder.Services.AddHostedService<OpenIdWorker>();
builder.Services.AddOptions<OpenIdWorkerSettings>().BindConfiguration("OpenIdWorkerSettings");

// TODO : Lock this down
builder.Services.AddCors(o =>
{
    o.AddDefaultPolicy(p =>
    {
        p.AllowAnyHeader();
        p.AllowAnyMethod();
        p.AllowAnyOrigin();
    });
});

builder.Services.AddQuartz(o =>
{
    o.UseSimpleTypeLoader();
    o.UseInMemoryStore();
}).AddQuartzHostedService(o => o.WaitForJobsToComplete = true);

builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString)
    .AddApplicationStatus()
    .AddApplicationInsightsPublisher(builder.Configuration["ApplicationInsights:ConnectionString"]);

builder.Services.AddApiVersioning(o =>
{
    o.ApiVersionReader = new MediaTypeApiVersionReader("api-v");
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1);
}).AddApiExplorer();

builder.Logging.AddApplicationInsights(
    configureTelemetryConfiguration: (config) => config.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"],
    configureApplicationInsightsLoggerOptions: (options) => { });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();

app.MapHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapStaticAssets();
app.MapControllers();

app.UseExceptionHandler(o =>
{
    o.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error is ValidationException validationException)
        {
            context.Response.StatusCode = 400;

            var errors = new Dictionary<string, string[]>();

            foreach (var error in validationException.Errors)
            {
                errors[error.PropertyName] = errors.TryGetValue(error.PropertyName, out var messages) ? [.. messages, error.ErrorMessage] : [error.ErrorMessage];
            }

            await context.Response.WriteAsJsonAsync(new ValidationProblemDetails(errors));
        }
        else if (exceptionHandlerPathFeature?.Error is ItemNotFoundException)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Item not found",
                Detail = "The requested item was not found.",
            });
        }
        else
        {
            // Let the framework handle all other exceptions
            throw exceptionHandlerPathFeature?.Error ?? new Exception("Unknown exception occurred.");
        }
    });
});

await app.RunAsync();
