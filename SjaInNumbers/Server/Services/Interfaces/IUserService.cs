using SjaInNumbers.Shared.Model.Users;

namespace SjaInNumbers.Server.Services.Interfaces;

public interface IUserService
{
    Task<bool> ApproveUserAsync(string userId);
    Task DeleteUserAsync(string userId);
    IAsyncEnumerable<UserDetails> GetAll();
    Task<UserDetails?> GetUserAsync(string userId);
    Task<bool> UpdateUserAsync(UserRoleChange userDetails);
}
