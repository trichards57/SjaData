using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SjaInNumbers.Shared.Model.Deployments;

public readonly record struct NationalPeakLoads : IDateMarked
{
    public IEnumerable<PeakLoads> Loads { get; init; }
    public DateTimeOffset LastModified { get; init; }
    public string ETag { get; init; }
}
