using SjaData.Server.Model.People;
using SJAData.Client.Services.Interfaces;

namespace SJAData.Services.Interfaces;

public interface ILocalPersonService : IPersonService
{
    Task<int> AddPeopleAsync(IAsyncEnumerable<PersonFileLine> people, string userId);
}
