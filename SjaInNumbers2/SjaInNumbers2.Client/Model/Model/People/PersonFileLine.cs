// <copyright file="PersonFileLine.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers2.Model.People;

/// <summary>
/// Represents a line in the person file.
/// </summary>
public class PersonFileLine
{
    /// <summary>
    /// Gets or sets the person's MyData number.
    /// </summary>
    public int MyDataNumber { get; set; }

    /// <summary>
    /// Gets or sets the person's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the person's name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the person's status.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the person's job role title.
    /// </summary>
    public string JobRoleTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the team or unit the person is in.
    /// </summary>
    public string TeamUnit { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the district or station the person is in.
    /// </summary>
    public string DistrictStation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the department or region the person is in.
    /// </summary>
    public string DepartmentRegion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the network the person is in.
    /// </summary>
    public string Network { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the peron's email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the person is a volunteer.
    /// </summary>
    public bool IsVolunteer => Status.Equals("volunteer", StringComparison.InvariantCultureIgnoreCase);
}
