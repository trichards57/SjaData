// <copyright file="PersonMap.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using CsvHelper.Configuration;

namespace SjaData.Server.Model;

internal class PersonMap : ClassMap<Person>
{
    public PersonMap()
    {
        Map(p => p.MyDataNumber).Name("MYDATA NUMBER");
        Map(p => p.FirstName).Name("First Name");
        Map(p => p.Name).Name("Name");
        Map(p => p.Status).Name("Status");
        Map(p => p.JobRoleTitle).Name("Job Role Title");
        Map(p => p.TeamUnit).Name("Team Unit");
        Map(p => p.DistrictStation).Name("District Station");
        Map(p => p.DepartmentRegion).Name("Department/Region");
        Map(p => p.Network).Name("Network");
        Map(p => p.Email).Name("E-Mail");
    }
}
