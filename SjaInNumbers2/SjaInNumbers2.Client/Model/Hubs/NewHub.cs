// <copyright file="NewHub.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SjaInNumbers2.Client.Model.Hubs;

public readonly record struct NewHub
{
    public int DistrictId { get; init; }

    [Required(AllowEmptyStrings = false)]
    public string Name { get; init; }
}
