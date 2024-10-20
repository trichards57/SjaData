﻿// <copyright file="UserService.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using SjaInNumbers.Client.Services.Interfaces;
using SjaInNumbers.Shared.Model.Users;
using System.Net.Http.Json;

namespace SjaInNumbers.Client.Services;

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

    public async Task<UserDetails> GetCurrentUserAsync()
    {
        return await client.GetFromJsonAsync<UserDetails>($"api/user/me");
    }
}
