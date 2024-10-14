using SjaInNumbers.Client.Services.Interfaces;
using SjaInNumbers.Server.Model.Hours;
using SjaInNumbers.Shared.Model;

namespace SjaInNumbers.Server.Services.Interfaces;

public interface ILocalHoursService : IHoursService
{
    /// <summary>
    /// Calculates the ETag associated with an hours count.
    /// </summary>
    /// <param name="date">The date of the report.</param>
    /// <param name="dateType">The date type for the report.</param>
    /// <param name="future">Indicates if only future information should be included.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the ETag.
    /// </returns>
    /// <remarks>
    /// This is a weak ETag and should be marked accordingly.
    /// </remarks>
    Task<string> GetHoursCountEtagAsync(DateOnly date, DateType dateType, bool future);

    /// <summary>
    /// Gets the date the hours data was last modified.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the date.
    /// </returns>
    Task<DateTimeOffset> GetLastModifiedAsync();

    Task<string> GetNhseTargetEtagAsync();

    Task<DateTimeOffset> GetNhseTargetLastModifiedAsync();

    Task<int> AddHours(IAsyncEnumerable<HoursFileLine> hours, string userId);

    /// <summary>
    /// Calculates the ETag associated with an activity report.
    /// </summary>
    /// <param name="region">The region to query for.</param>
    /// <param name="nhse">Indicates that only NSHE data should be returned.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. Resolves to the ETag.
    /// </returns>
    Task<string> GetTrendsEtagAsync(Region region, bool nhse);
}
