using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SjaInNumbers2.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore(c =>
{
    c.AddPolicy("Approved", o => o.RequireAuthenticatedUser().RequireClaim("Approved", "Yes"));
    c.AddPolicy("Admin", o => o.RequireRole("Admin").RequireClaim("Approved", "Yes"));
    c.AddPolicy("Lead", o => o.RequireRole("Admin", "Lead").RequireClaim("Approved", "Yes"));
});

await builder.Build().RunAsync();
