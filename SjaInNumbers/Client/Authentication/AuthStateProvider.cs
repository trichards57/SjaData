using Microsoft.AspNetCore.Components.Authorization;
using SjaInNumbers.Shared.Model.Users;
using System.Net.Http.Json;
using System.Security.Claims;

namespace SjaInNumbers.Client.Authentication;

public class AuthStateProvider(HttpClient client) : AuthenticationStateProvider
{
    private readonly HttpClient client = client;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var authState = await client.GetFromJsonAsync<UserDetails>("/api/user/me");

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(
                [new(ClaimTypes.NameIdentifier, authState.Id.ToString()),
                 new(ClaimTypes.Email, authState.Email),
                 new("Approved", authState.IsApproved ? "Yes" : "No"),
                 ..authState.Roles.Select(r => new Claim(ClaimTypes.Role, string.Join(",", r)))
                 ], "Microsoft", ClaimTypes.Email, ClaimTypes.Role)));
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            else
            {
                throw;
            }
        }
    }
}
