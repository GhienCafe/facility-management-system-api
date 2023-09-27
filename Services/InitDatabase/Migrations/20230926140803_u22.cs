using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceScheduleConfigs_AssetTypes_AssetTypeId",
                table: "MaintenanceScheduleConfigs");

            migrationBuilder.RenameColumn(
                name: "AssetTypeId",
                table: "MaintenanceScheduleConfigs",
                newName: "AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceScheduleConfigs_AssetTypeId",
                table: "MaintenanceScheduleConfigs",
                newName: "IX_MaintenanceScheduleConfigs_AssetId");

            migrationBuilder.AddColumn<int>(
                name: "LimitTimes",
                table: "Tokens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRented",
                table: "Assets",
                type: "bit",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceScheduleConfigs_Assets_AssetId",
                table: "MaintenanceScheduleConfigs",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceScheduleConfigs_Assets_AssetId",
                table: "MaintenanceScheduleConfigs");

            migrationBuilder.DropColumn(
                name: "LimitTimes",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "IsRented",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "AssetId",
                table: "MaintenanceScheduleConfigs",
                newName: "AssetTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceScheduleConfigs_AssetId",
                table: "MaintenanceScheduleConfigs",
                newName: "IX_MaintenanceScheduleConfigs_AssetTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceScheduleConfigs_AssetTypes_AssetTypeId",
                table: "MaintenanceScheduleConfigs",
                column: "AssetTypeId",
                principalTable: "AssetTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
