using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u70 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryDetailConfig_InventoryCheckConfig_InventoryConfigId",
                table: "InventoryDetailConfig");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryDetailConfig",
                table: "InventoryDetailConfig");

            migrationBuilder.RenameTable(
                name: "InventoryDetailConfig",
                newName: "InventoryDetailConfigs");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryDetailConfig_InventoryConfigId",
                table: "InventoryDetailConfigs",
                newName: "IX_InventoryDetailConfigs_InventoryConfigId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryDetailConfigs",
                table: "InventoryDetailConfigs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryDetailConfigs_InventoryCheckConfig_InventoryConfigId",
                table: "InventoryDetailConfigs",
                column: "InventoryConfigId",
                principalTable: "InventoryCheckConfig",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryDetailConfigs_InventoryCheckConfig_InventoryConfigId",
                table: "InventoryDetailConfigs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryDetailConfigs",
                table: "InventoryDetailConfigs");

            migrationBuilder.RenameTable(
                name: "InventoryDetailConfigs",
                newName: "InventoryDetailConfig");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryDetailConfigs_InventoryConfigId",
                table: "InventoryDetailConfig",
                newName: "IX_InventoryDetailConfig_InventoryConfigId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryDetailConfig",
                table: "InventoryDetailConfig",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryDetailConfig_InventoryCheckConfig_InventoryConfigId",
                table: "InventoryDetailConfig",
                column: "InventoryConfigId",
                principalTable: "InventoryCheckConfig",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
