using SjaInNumbers.Shared.Model.Deployments;

namespace SjaInNumbers.Client.Services.Interfaces;

public interface IDeploymentService
{
    Task<NationalSummary> GetNationalSummary();
    IAsyncEnumerable<PeakLoads> GetPeakLoads();
}
