using SjaData.Model.Hours;
using SjaData.Server.Api.Model;

namespace SjaData.Server.Services.Interfaces;

public interface IHoursService
{
    Task AddAsync(NewHoursEntry hours);

    Task<HoursCount> CountAsync(HoursQuery query);

    Task DeleteAsync(int id);

    Task<DateTimeOffset> GetLastModifiedAsync();
}
