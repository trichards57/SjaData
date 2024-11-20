using SjaInNumbers2.Client.Model.Deployments;

namespace SjaInNumbers2.Client.Services.Interfaces;

public interface IDeploymentService
{
    Task<NationalSummary> GetNationalSummary();
    IAsyncEnumerable<PeakLoads> GetPeakLoads();
}
