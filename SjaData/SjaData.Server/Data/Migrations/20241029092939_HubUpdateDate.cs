﻿// <copyright file="20241029092939_HubUpdateDate.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SjaData.Server.Migrations;

/// <inheritdoc />
public partial class HubUpdateDate : Migration
{
    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "Hubs");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Hubs",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(100)",
            oldMaxLength: 100);
    }

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Hubs",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "UpdatedAt",
            table: "Hubs",
            type: "datetimeoffset",
            nullable: false,
            defaultValue: new DateTimeOffset(new DateTime(2024, 10, 29, 9, 0, 0, 0, DateTimeKind.Utc), new TimeSpan(0, 0, 0, 0, 0)));
    }
}
