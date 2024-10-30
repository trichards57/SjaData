using SjaInNumbers.Server.Model.Deployments;

namespace SjaInNumbers.Server.Services.Interfaces;

public interface IDeploymentService
{
    Task AddDeploymentAsync(NewDeployment deployment);
    IAsyncEnumerable<DeploymentSummary> GetAllAsync(DateOnly startDate, DateOnly endDate);
    Task<NationalSummary> GetNationalSummaryAsync(DateOnly startDate, DateOnly endDate);
}
