// <copyright file="20241116110611_DistrictNameHistory.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SjaData.Server.Migrations;

/// <inheritdoc />
public partial class DistrictNameHistory : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "LastModified",
            table: "Districts",
            type: "datetimeoffset",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

        migrationBuilder.CreateTable(
            name: "DistrictPreviousName",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                DistrictId = table.Column<int>(type: "int", nullable: false),
                OldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DistrictPreviousName", x => x.Id);
                table.ForeignKey(
                    name: "FK_DistrictPreviousName_Districts_DistrictId",
                    column: x => x.DistrictId,
                    principalTable: "Districts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_DistrictPreviousName_DistrictId",
            table: "DistrictPreviousName",
            column: "DistrictId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DistrictPreviousName");

        migrationBuilder.DropColumn(
            name: "LastModified",
            table: "Districts");
    }
}
