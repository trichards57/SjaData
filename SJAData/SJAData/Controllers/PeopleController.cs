using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SjaData.Server.Model;
using SjaData.Server.Model.People;
using SJAData.Services.Interfaces;
using System.Globalization;
using System.Security.Claims;

namespace SJAData.Controllers;

[ApiController]
[Route("api/people")]
public class PeopleController(ILocalPersonService personService) : ControllerBase
{
    private readonly ILocalPersonService personService = personService;

    [HttpPost]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CountResponse), StatusCodes.Status200OK)]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> ReceivePersonFile(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.CurrentUICulture);
        csv.Context.RegisterClassMap<PersonFileLineMap>();

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updatedCount = await personService.AddPeopleAsync(csv.GetRecordsAsync<PersonFileLine>(), userId);

            return Ok(new CountResponse { Count = updatedCount });
        }
        catch (CsvHelperException)
        {
            return Problem("The uploaded CSV data was invalid.", statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
