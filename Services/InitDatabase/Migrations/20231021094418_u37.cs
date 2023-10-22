using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u37 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Extensions",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "AssetCheckId",
                table: "MediaFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MaintenanceId",
                table: "MediaFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RepairationId",
                table: "MediaFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReplacementId",
                table: "MediaFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TransportationId",
                table: "MediaFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "AssetTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_AssetCheckId",
                table: "MediaFiles",
                column: "AssetCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_MaintenanceId",
                table: "MediaFiles",
                column: "MaintenanceId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_RepairationId",
                table: "MediaFiles",
                column: "RepairationId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_ReplacementId",
                table: "MediaFiles",
                column: "ReplacementId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_TransportationId",
                table: "MediaFiles",
                column: "TransportationId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_AssetChecks_AssetCheckId",
                table: "MediaFiles",
                column: "AssetCheckId",
                principalTable: "AssetChecks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Maintenances_MaintenanceId",
                table: "MediaFiles",
                column: "MaintenanceId",
                principalTable: "Maintenances",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Repairations_RepairationId",
                table: "MediaFiles",
                column: "RepairationId",
                principalTable: "Repairations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Replacements_ReplacementId",
                table: "MediaFiles",
                column: "ReplacementId",
                principalTable: "Replacements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Transportations_TransportationId",
                table: "MediaFiles",
                column: "TransportationId",
                principalTable: "Transportations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_AssetChecks_AssetCheckId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Maintenances_MaintenanceId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Repairations_RepairationId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Replacements_ReplacementId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Transportations_TransportationId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_AssetCheckId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_MaintenanceId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_RepairationId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_ReplacementId",
                table: "MediaFiles");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_TransportationId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "AssetCheckId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "MaintenanceId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "RepairationId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "ReplacementId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "TransportationId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "AssetTypes");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Assets");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Extensions",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
