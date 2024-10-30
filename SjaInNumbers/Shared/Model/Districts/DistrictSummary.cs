using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SjaInNumbers.Shared.Model.Districts;

public readonly record struct DistrictSummary
{
    public int Id { get; init; }
    public string Name { get; init; }
    public Region Region { get; init; }
    public string Code { get; init; }
}
