using SjaData.Server.Model.Hours;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Services;

public class HoursService : IHoursService
{
    public Task AddAsync(NewHoursEntry hours)
    {
        throw new NotImplementedException();
    }

    public Task<HoursCount> CountAsync(HoursQuery query)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}
