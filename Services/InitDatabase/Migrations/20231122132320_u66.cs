using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u66 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CheckPeriod",
                table: "InventoryCheckConfigs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckDate",
                table: "InventoryCheckConfigs",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "InventoryCheckConfigs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckDate",
                table: "InventoryCheckConfigs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "InventoryCheckConfigs");

            migrationBuilder.AlterColumn<int>(
                name: "CheckPeriod",
                table: "InventoryCheckConfigs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
