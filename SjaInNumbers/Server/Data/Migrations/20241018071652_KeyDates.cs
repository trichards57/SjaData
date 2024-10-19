// <copyright file="20241018071652_KeyDates.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace SjaData.Server.Migrations;

/// <inheritdoc />
public partial class KeyDates : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "KeyDates",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                LastUpdateFile = table.Column<DateOnly>(type: "date", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_KeyDates", x => x.Id);
            });

        migrationBuilder.InsertData(
            table: "KeyDates",
            columns: ["Id", "LastUpdateFile"],
            values: [1, new DateOnly(1, 1, 1)]);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "KeyDates");
    }
}
