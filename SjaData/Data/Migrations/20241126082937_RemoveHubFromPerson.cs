using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SjaData.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHubFromPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "People",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                @"UPDATE People
                  SET DistrictId = Hubs.DistrictId
                  FROM People
                  INNER JOIN Hubs ON People.HubId = Hubs.Id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_People_DistrictId",
                table: "People",
                column: "DistrictId");

            migrationBuilder.AddForeignKey(
                name: "FK_People_Districts_DistrictId",
                table: "People",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id");

            migrationBuilder.DropIndex(
                name: "IX_People_HubId",
                table: "People");

            migrationBuilder.DropForeignKey(
                name: "FK_People_Hubs_HubId",
                table: "People");

            migrationBuilder.DropColumn(
                name: "HubId",
                table: "People");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_Districts_DistrictId",
                table: "People");

            migrationBuilder.DropIndex(
                name: "IX_People_DistrictId",
                table: "People");

            migrationBuilder.DropColumn(
                name: "DistrictId",
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
    }
}
