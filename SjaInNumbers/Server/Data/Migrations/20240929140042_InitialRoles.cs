// <copyright file="20240929140042_InitialRoles.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SjaData.Server.Migrations;

/// <inheritdoc />
public partial class InitialRoles : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData(
            table: "AspNetRoles",
            columns: ["Id", "ConcurrencyStamp", "Name", "NormalizedName"],
            values: new object[,]
            {
                { "1", null, "Admin", "ADMIN" },
                { "2", null, "Lead", "LEAD" },
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "1");

        migrationBuilder.DeleteData(
            table: "AspNetRoles",
            keyColumn: "Id",
            keyValue: "2");
    }
}
