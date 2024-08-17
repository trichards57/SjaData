using SjaData.Model.Hours;

namespace SjaData.Server.Services.Interfaces;

public interface IHoursService
{
    Task AddAsync(NewHoursEntry hours);
    Task<HoursCount> CountAsync(HoursQuery query);
    Task DeleteAsync(int id);
    Task<DateTimeOffset> GetLastModifiedAsync();
}
