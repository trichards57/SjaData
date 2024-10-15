using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SjaInNumbers.Client;
using SjaInNumbers.Client.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore(c =>
{
    c.AddPolicy("Approved", o => o.RequireAuthenticatedUser().RequireClaim("Approved", "Yes"));
});

//builder.Services.AddAuthorizationBuilder()
//    .AddPolicy("Approved", o => o.AddRequirements(new RequireApprovalRequirement()))
//    .AddPolicy("Admin", o => o.RequireRole("Admin").AddRequirements(new RequireApprovalRequirement()))
//    .AddPolicy("Lead", o => o.RequireRole("Admin", "Lead").AddRequirements(new RequireApprovalRequirement()));


builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddHttpClient("SjaInNumbers.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("SjaInNumbers.ServerAPI"));

await builder.Build().RunAsync();
