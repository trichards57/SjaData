// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using HealthChecks.ApplicationStatus.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SjaData.Authorization;
using SjaData.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.EnableEnrichment();

builder.Services.AddApplicationMetadata(x =>
{
    x.ApplicationName = "SJA in Numbers";
    x.EnvironmentName = builder.Environment.EnvironmentName;
    x.BuildVersion = typeof(Program).Assembly.GetName().Version?.ToString();
});

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

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

builder.Services.AddApplicationInsightsTelemetry(o =>
{
    if (builder.Environment.IsDevelopment())
    {
        o.DeveloperMode = true;
    }

    o.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});

builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString)
    .AddApplicationStatus()
    .AddApplicationInsightsPublisher(builder.Configuration["ApplicationInsights:ConnectionString"]);

builder.Services.AddScoped<IAuthorizationHandler, RequireApprovalHandler>();

builder.Services.AddRazorPages();

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
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapHealthChecks("/health");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

await app.RunAsync();
