using SjaData.Server.Model.Users;

namespace SjaData.Server.Services.Interfaces;

public interface IUserService
{
    IAsyncEnumerable<UserDetails> GetAll();
}
