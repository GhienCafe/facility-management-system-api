using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u31 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssetType",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetType",
                table: "Replacements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Replacements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetType",
                table: "Repairations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Repairations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetType",
                table: "Maintenances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Maintenances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetType",
                table: "AssetChecks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "AssetChecks",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetType",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "AssetType",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "AssetType",
                table: "Repairations");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Repairations");

            migrationBuilder.DropColumn(
                name: "AssetType",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "AssetType",
                table: "AssetChecks");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "AssetChecks");
        }
    }
}
