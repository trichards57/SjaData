using SjaInNumbers.Shared.Model;

namespace SjaInNumbers.Client.Converters;

public static class RegionSorter
{
    public static int SortRegion(Region region) => region switch
    {
        Region.NorthEast => 1,
        Region.NorthWest => 2,
        Region.EastMidlands => 3,
        Region.WestMidlands => 4,
        Region.EastOfEngland => 5,
        Region.London => 6,
        Region.SouthEast => 7,
        Region.SouthWest => 8,
        _ => 9,
    };
}
