using Microsoft.Extensions.Primitives;
using SjaData.Server.Model.People;
using SJAData.Client.Data;
using SJAData.Client.Services.Interfaces;

namespace SJAData.Services.Interfaces;

public interface ILocalPersonService : IPersonService
{
    Task<int> AddPeopleAsync(IAsyncEnumerable<PersonFileLine> people, string userId);
    Task<DateTimeOffset?> GetLastModifiedAsync();
    Task<string> GetPeopleReportsEtagAsync(DateOnly date, Region region);
}
