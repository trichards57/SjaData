// <copyright file="20241020180525_AddHubToPerson.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SjaData.Server.Migrations;

/// <inheritdoc />
public partial class AddHubToPerson : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "HubId",
            table: "People",
            type: "int",
            nullable: true);

        migrationBuilder.Sql(@"
                -- Insert new Districts based on existing People data where District is not null
                INSERT INTO Districts (Name, Region)
                SELECT DISTINCT District, Region
                FROM People
                WHERE District IS NOT NULL;

                -- Insert new Hubs based on the new Districts
                INSERT INTO Hubs (Name, DistrictId)
                SELECT DISTINCT 'AutoCreatedHubFor_' + Districts.Name, Districts.Id
                FROM Districts
                JOIN People ON People.District = Districts.Name AND People.Region = Districts.Region
                WHERE People.District IS NOT NULL AND People.Region IS NOT NULL;

                -- Update each Person to associate them with the appropriate Hub
                UPDATE People
                SET HubId = (
                    SELECT TOP 1 Hubs.Id
                    FROM Hubs
                    JOIN Districts ON Hubs.DistrictId = Districts.Id
                    WHERE People.District = Districts.Name AND People.Region = Districts.Region
                )
                WHERE People.District IS NOT NULL AND People.Region IS NOT NULL;
            ");

        migrationBuilder.DropColumn(
            name: "District",
            table: "People");

        migrationBuilder.DropColumn(
            name: "Region",
            table: "People");

        migrationBuilder.CreateIndex(
            name: "IX_People_HubId",
            table: "People",
            column: "HubId");

        migrationBuilder.AddForeignKey(
            name: "FK_People_Hubs_HubId",
            table: "People",
            column: "HubId",
            principalTable: "Hubs",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "District",
            table: "People",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: string.Empty);

        migrationBuilder.AddColumn<byte>(
            name: "Region",
            table: "People",
            type: "tinyint",
            nullable: false,
            defaultValue: (byte)0);

        migrationBuilder.Sql(@"
                -- Reverse the data migration process by restoring the district and region
                UPDATE People
                SET District = (SELECT TOP 1 Name FROM District WHERE HubId = People.HubId),
                    Region = (SELECT TOP 1 Region FROM District WHERE HubId = People.HubId)
                WHERE People.HubId IS NOT NULL;
            ");

        migrationBuilder.DropForeignKey(
            name: "FK_People_Hubs_HubId",
            table: "People");

        migrationBuilder.DropIndex(
            name: "IX_People_HubId",
            table: "People");

        migrationBuilder.DropColumn(
            name: "HubId",
            table: "People");
    }
}
