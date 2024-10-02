using SJAData.Client.Model;
using SJAData.Client.Services.Interfaces;
using SJAData.Model.Hours;

namespace SJAData.Services.Interfaces;

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
}
