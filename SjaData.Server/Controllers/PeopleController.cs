using Asp.Versioning;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SjaData.Server.Model;
using SjaData.Server.Services.Interfaces;
using System.Globalization;

namespace SjaData.Server.Controllers;

/// <summary>
/// Controller for managing people.
/// </summary>
/// <param name="personService">Service to manage people.</param>
[Route("api/people")]
[ApiController]
[ApiVersion("1.0")]
[Authorize]
public class PeopleController(IPersonService personService) : ControllerBase
{
    private readonly IPersonService personService = personService;

    /// <summary>
    /// Accepts a CSV file containing person data and adds the people to the database.
    /// </summary>
    /// <param name="fileData">The CSV file to accept.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Resolves to the outcome of the action.</returns>
    /// <response code="204">The people file was accepted successfully.</response>
    /// <response code="400">The request was invalid.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Consumes("text/csv")]
    public async Task<IActionResult> ReceivePersonFile([FromBody]string fileData)
    {
        using var reader = new StringReader(fileData);
        using var csv = new CsvReader(reader, CultureInfo.CurrentUICulture);
        csv.Context.RegisterClassMap<PersonMap>();

        try
        {
            await personService.AddPeople(csv.GetRecordsAsync<Person>());

            return NoContent();
        }
        catch (CsvHelperException)
        {
            return Problem("The uploaded CSV data was invalid.", statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
