using Microsoft.AspNetCore.Mvc;
using SjaData.Model.Hours;
using SjaData.Server.Services.Interfaces;

namespace SjaData.Server.Api;

public static class HoursApiExtensions
{
    public static WebApplication MapHoursApi(this WebApplication webApp)
    {
        var group = webApp.MapGroup("/api/hours");

        group.MapPost(string.Empty, async ([FromBody] NewHoursEntry hours, [FromServices] IHoursService hoursService, HttpContext context) =>
        {
            await hoursService.AddAsync(hours);
            return Results.NoContent();
        });

        group.MapGet("count", async (HoursQuery query, [FromServices] IHoursService hoursService, HttpContext context) =>
        {
            if (context.Request.GetTypedHeaders().IfModifiedSince.HasValue)
            {
                var age = await hoursService.GetLastModifiedAsync() - context.Request.GetTypedHeaders().IfModifiedSince;

                if (age < TimeSpan.FromSeconds(1))
                {
                    return Results.StatusCode(StatusCodes.Status304NotModified);
                }
            }

            var count = await hoursService.CountAsync(query);

            context.Response.GetTypedHeaders().LastModified = count.LastUpdate;
            context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue { Private = true, NoCache = true };

            return Results.Ok(count);
        });

        group.MapDelete("{id}", async ([FromRoute] int id, [FromServices] IHoursService hoursService) =>
        {
            await hoursService.DeleteAsync(id);
            return Results.NoContent();
        });

        return webApp;
    }
}
