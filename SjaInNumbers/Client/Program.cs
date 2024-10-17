// <copyright file="Program.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SjaInNumbers.Client;
using SjaInNumbers.Client.Authentication;
using SjaInNumbers.Client.Services;
using SjaInNumbers.Client.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore(c =>
{
    c.AddPolicy("Approved", o => o.RequireAuthenticatedUser().RequireClaim("Approved", "Yes"));
    c.AddPolicy("Admin", o => o.RequireRole("Admin").RequireClaim("Approved", "Yes"));
    c.AddPolicy("Lead", o => o.RequireRole("Admin", "Lead").RequireClaim("Approved", "Yes"));
});

builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddHttpClient("SjaInNumbers.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IHoursService, HoursService>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("SjaInNumbers.ServerAPI"));

await builder.Build().RunAsync();
