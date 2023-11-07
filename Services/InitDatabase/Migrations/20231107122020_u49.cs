using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u49 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "TransportationDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Tokens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Teams",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "TeamMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "RoomTypes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "RoomStatus",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Rooms",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "RoomAssets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Replacements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Repairations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Models",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "MediaFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Maintenances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "InventoryChecks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "InventoryCheckDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "InventoryCheckConfigs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Floors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Campuses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Buildings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Brands",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "AssetTypes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Assets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "AssetChecks",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "TransportationDetails");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "TeamMembers");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "RoomStatus");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "RoomAssets");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Repairations");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "InventoryChecks");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "InventoryCheckDetails");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "InventoryCheckConfigs");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Floors");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Campuses");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "AssetTypes");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "AssetChecks");
        }
    }
}
