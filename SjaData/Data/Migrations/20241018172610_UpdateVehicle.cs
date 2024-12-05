// <copyright file="20241018172610_UpdateVehicle.cs" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

namespace SjaData.Server.Migrations;

/// <inheritdoc />
public partial class UpdateVehicle : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Vehicles_AspNetUsers_UpdatedById",
            table: "Vehicles");

        migrationBuilder.DropIndex(
            name: "IX_Vehicles_UpdatedById",
            table: "Vehicles");

        migrationBuilder.DropColumn(
            name: "UpdatedById",
            table: "Vehicles");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "UpdatedById",
            table: "Vehicles",
            type: "nvarchar(450)",
            nullable: false,
            defaultValue: string.Empty);

        migrationBuilder.CreateIndex(
            name: "IX_Vehicles_UpdatedById",
            table: "Vehicles",
            column: "UpdatedById");

        migrationBuilder.AddForeignKey(
            name: "FK_Vehicles_AspNetUsers_UpdatedById",
            table: "Vehicles",
            column: "UpdatedById",
            principalTable: "AspNetUsers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }
}
