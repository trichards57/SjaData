using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SjaInNumbers.Server.Services.Interfaces;
using SjaInNumbers.Shared.Model.Hubs;

namespace SjaInNumbers.Server.Controllers;
[Route("api/hubs")]
[ApiController]
public class HubsController(IHubService hubService) : ControllerBase
{
    private readonly IHubService hubService = hubService;

    [HttpGet]
    public IAsyncEnumerable<HubSummary> GetHubSummaries() => hubService.GetAllAsync();
}
