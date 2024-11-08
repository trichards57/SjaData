
using SjaInNumbers.Shared.Model.Districts;

namespace SjaInNumbers.Server.Services.Interfaces;

public interface IDistrictService
{
    IAsyncEnumerable<DistrictSummary> GetAll();
    Task<DistrictSummary?> GetDistrict(int id);
    Task<int?> GetIdByDistrictCodeAsync(string code);
    Task<bool> SetDistrictCodeAsync(int id, string code);
}
