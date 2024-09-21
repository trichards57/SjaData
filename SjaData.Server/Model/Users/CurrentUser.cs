namespace SjaData.Server.Model.Users;

public readonly record struct CurrentUser
{
    public string Name { get; init; }

    public string Role { get; init; }
}
