using SjaData.Server.Model;

namespace SjaData.Server.Services.Interfaces;

public interface IPersonService
{
    Task AddPeople(IAsyncEnumerable<Person> asyncEnumerable);
}
