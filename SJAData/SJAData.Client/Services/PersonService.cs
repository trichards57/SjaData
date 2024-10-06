using Microsoft.AspNetCore.WebUtilities;
using SJAData.Client.Data;
using SJAData.Client.Model.People;
using SJAData.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace SJAData.Client.Services;

public class PersonService(HttpClient httpClient) : IPersonService
{
    private readonly HttpClient httpClient = httpClient;

    public IAsyncEnumerable<PersonReport> GetPeopleReportsAsync(DateOnly date, Region region)
    {
        var uri = QueryHelpers.AddQueryString(
           "/api/people/reports",
           new Dictionary<string, string?>()
           {
                { "date", date.ToString("o") },
                { "region", region.ToString() },
           });

        return httpClient.GetFromJsonAsAsyncEnumerable<PersonReport>(uri);
    }
}
