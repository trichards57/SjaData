
using SjaInNumbers.Shared.Model.Districts;

namespace SjaInNumbers.Server.Services.Interfaces;

public interface IDistrictService
{
    IAsyncEnumerable<DistrictSummary> GetAll();
    Task<int?> GetByDistrictCodeAsync(string code);
}
