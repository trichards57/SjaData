﻿// <copyright file="20241029192409_Deployments.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SjaData.Server.Migrations;

/// <inheritdoc />
public partial class Deployments : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Deployments",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Date = table.Column<DateOnly>(type: "date", nullable: false),
                FrontLineAmbulances = table.Column<int>(type: "int", nullable: false),
                AllWheelDriveAmbulances = table.Column<int>(type: "int", nullable: false),
                OffRoadAmbulances = table.Column<int>(type: "int", nullable: false),
                DipsReference = table.Column<int>(type: "int", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Deployments", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Deployments");
    }
}