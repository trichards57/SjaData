// <copyright file="20241017164215_Vehicles.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SjaData.Server.Migrations;

/// <inheritdoc />
public partial class Vehicles : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Vehicles",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                BodyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CallSign = table.Column<string>(type: "nvarchar(450)", nullable: false),
                Deleted = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ForDisposal = table.Column<bool>(type: "bit", nullable: false),
                Hub = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsVor = table.Column<bool>(type: "bit", nullable: false),
                LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                Make = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Region = table.Column<byte>(type: "tinyint", nullable: false),
                Registration = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                VehicleType = table.Column<int>(type: "int", nullable: false),
                UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Vehicles", x => x.Id);
                table.ForeignKey(
                    name: "FK_Vehicles_AspNetUsers_UpdatedById",
                    column: x => x.UpdatedById,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "VehicleIncidents",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                EstimatedEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                VehicleId = table.Column<int>(type: "int", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_VehicleIncidents", x => x.Id);
                table.ForeignKey(
                    name: "FK_VehicleIncidents_Vehicles_VehicleId",
                    column: x => x.VehicleId,
                    principalTable: "Vehicles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_VehicleIncidents_VehicleId",
            table: "VehicleIncidents",
            column: "VehicleId");

        migrationBuilder.CreateIndex(
            name: "IX_Vehicles_CallSign",
            table: "Vehicles",
            column: "CallSign");

        migrationBuilder.CreateIndex(
            name: "IX_Vehicles_Registration",
            table: "Vehicles",
            column: "Registration",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Vehicles_UpdatedById",
            table: "Vehicles",
            column: "UpdatedById");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "VehicleIncidents");

        migrationBuilder.DropTable(
            name: "Vehicles");
    }
}
