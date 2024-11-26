namespace SjaInNumbers.Shared.Model;

/// <summary>
/// Represents a data item that has a last modified date and an ETag.
/// </summary>
public interface IDateMarked
{
    /// <summary>
    /// Gets the date the data was last modified.
    /// </summary>
    DateTimeOffset LastModified { get; }

    /// <summary>
    /// Gets the ETag for the data.
    /// </summary>
    string ETag { get; }
}
