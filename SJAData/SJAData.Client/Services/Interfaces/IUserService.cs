using SJAData.Client.Model.Users;

namespace SJAData.Client.Services.Interfaces;

public interface IUserService
{
    Task<bool> ApproveUserAsync(string userId);
    Task DeleteUserAsync(string userId);
    IAsyncEnumerable<UserDetails> GetAll();

    Task<bool> UpdateUserAsync(UserRoleChange userDetails);
}
