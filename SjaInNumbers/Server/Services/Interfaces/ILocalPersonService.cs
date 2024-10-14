using Microsoft.Extensions.Primitives;
using SjaInNumbers.Client.Services.Interfaces;
using SjaInNumbers.Server.Model.People;
using SjaInNumbers.Shared.Model;

namespace SjaInNumbers.Server.Services.Interfaces;

public interface ILocalPersonService : IPersonService
{
    Task<int> AddPeopleAsync(IAsyncEnumerable<PersonFileLine> people, string userId);
    Task<DateTimeOffset?> GetLastModifiedAsync();
    Task<string> GetPeopleReportsEtagAsync(DateOnly date, Region region);
}
