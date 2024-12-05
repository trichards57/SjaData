// <copyright file="20241020174037_SavedPlaces.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SjaData.Server.Migrations;

/// <inheritdoc />
public partial class SavedPlaces : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "District",
            table: "Vehicles");

        migrationBuilder.DropColumn(
            name: "Hub",
            table: "Vehicles");

        migrationBuilder.DropColumn(
            name: "Region",
            table: "Vehicles");

        migrationBuilder.AddColumn<int>(
            name: "HubId",
            table: "Vehicles",
            type: "int",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "Districts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Region = table.Column<byte>(type: "tinyint", nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Districts", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Hubs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                DistrictId = table.Column<int>(type: "int", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Hubs", x => x.Id);
                table.ForeignKey(
                    name: "FK_Hubs_Districts_DistrictId",
                    column: x => x.DistrictId,
                    principalTable: "Districts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Vehicles_HubId",
            table: "Vehicles",
            column: "HubId");

        migrationBuilder.CreateIndex(
            name: "IX_Hubs_DistrictId",
            table: "Hubs",
            column: "DistrictId");

        migrationBuilder.AddForeignKey(
            name: "FK_Vehicles_Hubs_HubId",
            table: "Vehicles",
            column: "HubId",
            principalTable: "Hubs",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Vehicles_Hubs_HubId",
            table: "Vehicles");

        migrationBuilder.DropTable(
            name: "Hubs");

        migrationBuilder.DropTable(
            name: "Districts");

        migrationBuilder.DropIndex(
            name: "IX_Vehicles_HubId",
            table: "Vehicles");

        migrationBuilder.DropColumn(
            name: "HubId",
            table: "Vehicles");

        migrationBuilder.AddColumn<string>(
            name: "District",
            table: "Vehicles",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: string.Empty);

        migrationBuilder.AddColumn<string>(
            name: "Hub",
            table: "Vehicles",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: string.Empty);

        migrationBuilder.AddColumn<byte>(
            name: "Region",
            table: "Vehicles",
            type: "tinyint",
            nullable: false,
            defaultValue: (byte)0);
    }
}
