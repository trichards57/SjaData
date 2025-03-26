// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SjaInNumbers.Client;
using SjaInNumbers.Client.Services;
using SjaInNumbers.Client.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Local", options.ProviderOptions);
    options.ProviderOptions.DefaultScopes.Add("roles");
    options.UserOptions.RoleClaim = "role";
});

builder.Services.AddAuthorizationCore(c =>
{
    c.AddPolicy("Approved", o => o.RequireAuthenticatedUser().RequireClaim("Approved", "True"));
    c.AddPolicy("Admin", o => o.RequireRole("Admin").RequireClaim("Approved", "True"));
    c.AddPolicy("Lead", o => o.RequireRole("Admin", "Lead").RequireClaim("Approved", "True"));
});

builder.Services.AddTransient<ApiAuthorizationMessageHandler>();

builder.Services.AddHttpClient("WebAPI", c => c.BaseAddress = new Uri(builder.Configuration.GetValue("ApiBase", "https://localhost:7191/")))
    .AddHttpMessageHandler<ApiAuthorizationMessageHandler>();
builder.Services.AddScoped(s => s.GetRequiredService<IHttpClientFactory>().CreateClient("WebAPI"));

builder.Services.AddScoped<IDeploymentService, DeploymentService>();
builder.Services.AddScoped<IDistrictsService, DistrictsService>();
builder.Services.AddScoped<IHoursService, HoursService>();
builder.Services.AddScoped<IHubService, HubsService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

await builder.Build().RunAsync();
