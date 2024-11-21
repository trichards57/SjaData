// <copyright file="DeploymentsFileLine.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace SjaInNumbers2.Model.Deployments;

/// <summary>
/// Represents a line in a deployments file.
/// </summary>
public class DeploymentsFileLine
{
    /// <summary>
    /// Gets or sets the ID of the deployment.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of the deployment.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Gets or sets the name of the deployment.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the DIPS reference of the deployment.
    /// </summary>
    public int? DipsNumber { get; set; }

    /// <summary>
    /// Gets or sets the start time of the deployment.
    /// </summary>
    public TimeOnly StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the deployment.
    /// </summary>
    public TimeOnly FinishTime { get; set; }

    /// <summary>
    /// Gets or sets the district of the deployment.
    /// </summary>
    public string District { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the approval stage the deployment is in.
    /// </summary>
    public string ApprovalStage { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of front-line ambulances at the deployment.
    /// </summary>
    public int Ambulances { get; set; }

    /// <summary>
    /// Gets or sets the number of blue-light driver EACs for the deployment.
    /// </summary>
    public int BlueLightEac { get; set; }

    /// <summary>
    /// Gets or sets the number of EACs for the deployment.
    /// </summary>
    public int Eacs { get; set; }

    /// <summary>
    /// Gets or sets the number of paramedics for the deployment.
    /// </summary>
    public int Paramedics { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the shifts have been created for the deployment.
    /// </summary>
    public bool ShiftsCreated { get; set; }

    /// <summary>
    /// Gets or sets the type of the deployment.
    /// </summary>
    public string TypeOfEvent { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the event lead responsible for the deployment.
    /// </summary>
    public string EventLeadResponsible { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ambulance lead responsible for the deployment.
    /// </summary>
    public string AmbulanceLead { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hub location for the deployment.
    /// </summary>
    public string HubLocation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of off-road ambulances for the deployment.
    /// </summary>
    public int OffRoadAmbulances { get; set; }

    /// <summary>
    /// Gets or sets the number of all-wheel drive ambulances for the deployment.
    /// </summary>
    public int AllWheelDriveAmbulances { get; set; }

    /// <summary>
    /// Gets or sets the notes for the deployment.
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the requesting person.
    /// </summary>
    public string Requester { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date the deployment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date the deployment was accepted.
    /// </summary>
    public DateOnly DateAcceptedByLead { get; set; }

    /// <summary>
    /// Gets or sets the date the deployment was last modified.
    /// </summary>
    public DateTime Modified { get; set; }

    /// <summary>
    /// Gets or sets the item type.
    /// </summary>
    public string ItemType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the item.
    /// </summary>
    public string Path { get; set; } = string.Empty;
}
