using SJAData.Client.Model;
using SJAData.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace SJAData.Client.Services;

internal class HoursService(HttpClient httpClient) : IHoursService
{
    private readonly HttpClient httpClient = httpClient;

    //public Task<HoursCountResponse> CountAsync(DateOnly? date, DateType? dateType, bool future)
    //{
    //    throw new NotImplementedException();
    //}

    //public Task DeleteAsync(int id)
    //{
    //    throw new NotImplementedException();
    //}

    //public Task<string> GetHoursCountEtagAsync(DateOnly date, DateType dateType, bool future)
    //{
    //    throw new NotImplementedException();
    //}

    //public Task<DateTimeOffset> GetLastModifiedAsync()
    //{
    //    throw new NotImplementedException();
    //}

    public async Task<int> GetNhseTargetAsync()
    {
        var response = await httpClient.GetFromJsonAsync<HoursTarget>("/api/hours/target");

        return response.Target;
    }

    //public Task<HoursTrendsResponse> GetTrendsAsync(Region region, bool nhse)
    //{
    //    throw new NotImplementedException();
    //}

    //public Task<string> GetTrendsEtagAsync(Region region, bool nhse)
    //{
    //    throw new NotImplementedException();
    //}
}
