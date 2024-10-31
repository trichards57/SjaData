using SjaInNumbers.Client.Services.Interfaces;
using SjaInNumbers.Shared.Model.Deployments;
using System.Net.Http.Json;

namespace SjaInNumbers.Client.Services;

public class DeploymentService(HttpClient client) : IDeploymentService
{
    private readonly HttpClient client = client;

    public IAsyncEnumerable<PeakLoads> GetPeakLoads()
    {
        return client.GetFromJsonAsAsyncEnumerable<PeakLoads>("/api/deployments/peaks");
    }
}
