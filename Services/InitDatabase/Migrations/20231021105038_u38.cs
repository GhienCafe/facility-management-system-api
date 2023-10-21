using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u38 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceScheduleConfigs_Assets_AssetId",
                table: "MaintenanceScheduleConfigs");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Replacements_ReplacementId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_ReplacementId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceScheduleConfigs_AssetId",
                table: "MaintenanceScheduleConfigs");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "MaintenanceScheduleConfigs");

            migrationBuilder.AddColumn<Guid>(
                name: "MaintenanceConfigId",
                table: "Assets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_MaintenanceConfigId",
                table: "Assets",
                column: "MaintenanceConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_MaintenanceScheduleConfigs_MaintenanceConfigId",
                table: "Assets",
                column: "MaintenanceConfigId",
                principalTable: "MaintenanceScheduleConfigs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Replacements_RepairationId",
                table: "MediaFiles",
                column: "RepairationId",
                principalTable: "Replacements",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_MaintenanceScheduleConfigs_MaintenanceConfigId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Replacements_RepairationId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_Assets_MaintenanceConfigId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "MaintenanceConfigId",
                table: "Assets");

            migrationBuilder.AddColumn<Guid>(
                name: "AssetId",
                table: "MaintenanceScheduleConfigs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_ReplacementId",
                table: "MediaFiles",
                column: "ReplacementId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceScheduleConfigs_AssetId",
                table: "MaintenanceScheduleConfigs",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceScheduleConfigs_Assets_AssetId",
                table: "MaintenanceScheduleConfigs",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Replacements_ReplacementId",
                table: "MediaFiles",
                column: "ReplacementId",
                principalTable: "Replacements",
                principalColumn: "Id");
        }
    }
}
