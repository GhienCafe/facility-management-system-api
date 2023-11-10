using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u58 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryChecks_InventoryCheckConfigs_InventoryCheckConfigId",
                table: "InventoryChecks");

            migrationBuilder.AlterColumn<Guid>(
                name: "InventoryCheckConfigId",
                table: "InventoryChecks",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "InventoryCheckDetails",
                type: "float",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryChecks_InventoryCheckConfigs_InventoryCheckConfigId",
                table: "InventoryChecks",
                column: "InventoryCheckConfigId",
                principalTable: "InventoryCheckConfigs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryChecks_InventoryCheckConfigs_InventoryCheckConfigId",
                table: "InventoryChecks");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "InventoryCheckDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "InventoryCheckConfigId",
                table: "InventoryChecks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryChecks_InventoryCheckConfigs_InventoryCheckConfigId",
                table: "InventoryChecks",
                column: "InventoryCheckConfigId",
                principalTable: "InventoryCheckConfigs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
