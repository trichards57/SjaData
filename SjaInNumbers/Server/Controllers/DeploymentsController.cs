using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SjaInNumbers.Server.Model.People;
using SjaInNumbers.Server.Model;
using System.Globalization;
using System.Security.Claims;
using SjaInNumbers.Server.Model.Deployments;
using SjaInNumbers.Server.Services.Interfaces;

namespace SjaInNumbers.Server.Controllers;

[ApiController]
[Route("api/deployments")]
public class DeploymentsController( IDeploymentService deploymentService) : ControllerBase
{
    private readonly IDeploymentService deploymentService = deploymentService;

    [HttpPost]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CountResponse), StatusCodes.Status200OK)]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<CountResponse>> ReceiveDeploymentsFile(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.CurrentUICulture);
        csv.Context.RegisterClassMap<DeploymentsFileLineMap>();

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("Unable to get user details.");

            await foreach (var record in csv.GetRecordsAsync<DeploymentsFileLine>())
            {
                var district = 

                var item = new NewDeployment
                {
                    AllWheelDriveAmbulances = record.AllWheelDriveAmbulances,
                    OffRoadAmbulances = record.OffRoadAmbulances,
                    Date = record.Date,
                    DipsReference = record.DipsNumber ?? 0,
                    FrontLineAmbulances = record.Ambulances,
                    Name = record.Name,
                };
            }

            var updatedCount = await deploymentService.AddDeploymentAsync(csv.GetRecordsAsync<PersonFileLine>(), userId);

            return Ok(new CountResponse { Count = updatedCount });
        }
        catch (CsvHelperException)
        {
            return Problem("The uploaded CSV data was invalid.", statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
