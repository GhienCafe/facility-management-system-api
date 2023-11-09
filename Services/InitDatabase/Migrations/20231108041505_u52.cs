using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u52 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCheckDetails_InventoryChecks_AssetId",
                table: "InventoryCheckDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_InventoryChecks_MaintenanceId",
                table: "MediaFiles");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_InventoryCheckId",
                table: "MediaFiles",
                column: "InventoryCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheckDetails_InventoryCheckId",
                table: "InventoryCheckDetails",
                column: "InventoryCheckId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCheckDetails_InventoryChecks_InventoryCheckId",
                table: "InventoryCheckDetails",
                column: "InventoryCheckId",
                principalTable: "InventoryChecks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_InventoryChecks_InventoryCheckId",
                table: "MediaFiles",
                column: "InventoryCheckId",
                principalTable: "InventoryChecks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCheckDetails_InventoryChecks_InventoryCheckId",
                table: "InventoryCheckDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_InventoryChecks_InventoryCheckId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_InventoryCheckId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_InventoryCheckDetails_InventoryCheckId",
                table: "InventoryCheckDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCheckDetails_InventoryChecks_AssetId",
                table: "InventoryCheckDetails",
                column: "AssetId",
                principalTable: "InventoryChecks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_InventoryChecks_MaintenanceId",
                table: "MediaFiles",
                column: "MaintenanceId",
                principalTable: "InventoryChecks",
                principalColumn: "Id");
        }
    }
}
