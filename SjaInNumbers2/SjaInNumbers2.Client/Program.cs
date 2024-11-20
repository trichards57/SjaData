// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SjaInNumbers2.Client;
using SjaInNumbers2.Client.Services;
using SjaInNumbers2.Client.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore(c =>
{
    c.AddPolicy("Approved", o => o.RequireAuthenticatedUser().RequireClaim("Approved", "Yes"));
    c.AddPolicy("Admin", o => o.RequireRole("Admin").RequireClaim("Approved", "Yes"));
    c.AddPolicy("Lead", o => o.RequireRole("Admin", "Lead").RequireClaim("Approved", "Yes"));
});

builder.Services.AddHttpClient("SjaInNumbers.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddScoped<IDeploymentService, DeploymentService>();
builder.Services.AddScoped<IDistrictsService, DistrictsService>();
builder.Services.AddScoped<IHoursService, HoursService>();
builder.Services.AddScoped<IHubService, HubsService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

await builder.Build().RunAsync();
