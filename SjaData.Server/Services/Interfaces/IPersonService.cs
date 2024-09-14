using SjaData.Server.Model;

namespace SjaData.Server.Services.Interfaces;

public interface IPersonService
{
    Task<int> AddPeople(IAsyncEnumerable<Person> asyncEnumerable);
}
