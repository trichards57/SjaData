using SjaInNumbers.Shared.Model.Deployments;

namespace SjaInNumbers.Client.Services.Interfaces;

public interface IDeploymentService
{
    IAsyncEnumerable<PeakLoads> GetPeakLoads();
}
