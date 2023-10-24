using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u39 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Checkin",
                table: "Transportations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Checkout",
                table: "Transportations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Checkin",
                table: "Replacements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Checkout",
                table: "Replacements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Checkin",
                table: "Repairations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Checkout",
                table: "Repairations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Checkin",
                table: "Maintenances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Checkout",
                table: "Maintenances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Checkin",
                table: "AssetChecks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Checkout",
                table: "AssetChecks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Checkin",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "Checkout",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "Checkin",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "Checkout",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "Checkin",
                table: "Repairations");

            migrationBuilder.DropColumn(
                name: "Checkout",
                table: "Repairations");

            migrationBuilder.DropColumn(
                name: "Checkin",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Checkout",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Checkin",
                table: "AssetChecks");

            migrationBuilder.DropColumn(
                name: "Checkout",
                table: "AssetChecks");
        }
    }
}
