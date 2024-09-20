using SjaData.Server.Data;

namespace SjaData.Server.Model.Users;

public readonly record struct UserDetails
{
    public string Id { get; init; }
    public string Name { get; init; }
    public Role Role { get; init; }
}
