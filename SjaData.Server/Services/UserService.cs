using Microsoft.EntityFrameworkCore;
using SjaData.Server.Data;
using SjaData.Server.Model.Users;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Services;

public class UserService(DataContext context) : IUserService
{
    private readonly DataContext context = context;

    /// <inheritdoc/>
    public IAsyncEnumerable<UserDetails> GetAll()
    {
        return context.Users.Select(u => new UserDetails { Id = u.Id, Name = u.Name, Role = u.Role }).AsAsyncEnumerable();
    }
}
