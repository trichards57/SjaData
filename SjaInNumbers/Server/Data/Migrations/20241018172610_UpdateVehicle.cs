using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SjaData.Server.Migrations
{
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
                defaultValue: "");

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
}
