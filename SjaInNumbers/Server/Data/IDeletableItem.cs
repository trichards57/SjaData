
namespace SjaInNumbers.Server.Data;

public interface IDeletableItem
{
    DateTimeOffset? Deleted { get; set; }
}
