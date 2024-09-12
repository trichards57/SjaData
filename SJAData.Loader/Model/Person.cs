// <copyright file="Person.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SJAData.Loader.Model;

internal class Person
{
    public int MyDataNumber { get; set; }

    public string FirstName { get; set; }

    public string Name { get; set; }

    public string Status { get; set; }

    public string JobRoleTitle { get; set; }

    public string TeamUnit { get; set; }

    public string DistrictStation { get; set; }

    public string DepartmentRegion { get; set; }

    public string Network { get; set; }

    public string Email { get; set; }

    public bool IsVolunteer => Status.Equals("volunteer", StringComparison.InvariantCultureIgnoreCase);
}
