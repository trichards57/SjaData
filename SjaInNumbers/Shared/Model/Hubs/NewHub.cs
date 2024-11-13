using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SjaInNumbers.Shared.Model.Hubs;

public readonly record struct NewHub
{
    public int DistrictId { get; init; }

    [Required(AllowEmptyStrings = false)]
    public string Name { get; init; }
}
