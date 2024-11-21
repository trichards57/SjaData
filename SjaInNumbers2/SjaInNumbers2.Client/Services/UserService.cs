// <copyright file="UserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers2.Client.Model.Users;
using SjaInNumbers2.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace SjaInNumbers2.Client.Services;

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

    public async Task<UserDetails?> GetUserAsync(string userId)
    {
        return await client.GetFromJsonAsync<UserDetails>($"api/user/{userId}");
    }
}
