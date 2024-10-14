using SjaInNumbers.Shared.Model.Users;

namespace SjaInNumbers.Client.Services.Interfaces;

public interface IUserService
{
    Task<bool> ApproveUserAsync(string userId);
    Task DeleteUserAsync(string userId);
    IAsyncEnumerable<UserDetails> GetAll();

    Task<bool> UpdateUserAsync(UserRoleChange userDetails);
}
