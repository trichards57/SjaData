using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SjaData.Model.Validation;

internal interface IRegionAndTrust
{
    Region Region { get; }
    Trust Trust { get; }
}
