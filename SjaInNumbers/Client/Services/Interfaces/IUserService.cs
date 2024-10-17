using SjaInNumbers.Shared.Model.Users;

namespace SjaInNumbers.Client.Services.Interfaces;

internal interface IUserService
{
    Task<bool> ApproveUserAsync(string userId);
    Task DeleteUserAsync(string userId);
    IAsyncEnumerable<UserDetails> GetAll();
    Task<UserDetails> GetCurrentUserAsync(string userId);
    Task<bool> UpdateUserAsync(UserRoleChange userDetails);
}
