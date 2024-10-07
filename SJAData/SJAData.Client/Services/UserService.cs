using SJAData.Client.Model.Users;
using SJAData.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace SJAData.Client.Services;

public class UserService(HttpClient client) : IUserService
{
    private readonly HttpClient client = client;

    public IAsyncEnumerable<UserDetails> GetAll()
    {
        return client.GetFromJsonAsAsyncEnumerable<UserDetails>("api/user");
    }

    public async Task<bool> UpdateUserAsync(UserRoleChange userDetails)
    {
        var result = await client.PostAsJsonAsync("api/user", userDetails);

        return result.IsSuccessStatusCode;
    }

    public async Task<bool> ApproveUserAsync(string userId)
    {
        var result = await client.PostAsJsonAsync($"api/user/{userId}/approve", new { });

        return result.IsSuccessStatusCode;
    }

    public async Task DeleteUserAsync(string userId)
    {
        await client.DeleteAsync($"api/user/{userId}");
    }
}
