using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u67 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryChecks_InventoryCheckConfigs_InventoryCheckConfigId",
                table: "InventoryChecks");

            migrationBuilder.DropIndex(
                name: "IX_InventoryChecks_InventoryCheckConfigId",
                table: "InventoryChecks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryCheckConfigs",
                table: "InventoryCheckConfigs");

            migrationBuilder.DropColumn(
                name: "InventoryCheckConfigId",
                table: "InventoryChecks");

            migrationBuilder.DropColumn(
                name: "CheckDate",
                table: "InventoryCheckConfigs");

            migrationBuilder.DropColumn(
                name: "CheckPeriod",
                table: "InventoryCheckConfigs");

            migrationBuilder.DropColumn(
                name: "LastCheckedDate",
                table: "InventoryCheckConfigs");

            migrationBuilder.RenameTable(
                name: "InventoryCheckConfigs",
                newName: "InventoryCheckConfig");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "InventoryCheckConfig",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "InventoryCheckConfig",
                newName: "Content");

            migrationBuilder.AddColumn<int>(
                name: "NotificationDays",
                table: "InventoryCheckConfig",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryCheckConfig",
                table: "InventoryCheckConfig",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "InventoryDetailConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventoryConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventoryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryDetailConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryDetailConfig_InventoryCheckConfig_InventoryConfigId",
                        column: x => x.InventoryConfigId,
                        principalTable: "InventoryCheckConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDetailConfig_InventoryConfigId",
                table: "InventoryDetailConfig",
                column: "InventoryConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryDetailConfig");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryCheckConfig",
                table: "InventoryCheckConfig");

            migrationBuilder.DropColumn(
                name: "NotificationDays",
                table: "InventoryCheckConfig");

            migrationBuilder.RenameTable(
                name: "InventoryCheckConfig",
                newName: "InventoryCheckConfigs");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "InventoryCheckConfigs",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "InventoryCheckConfigs",
                newName: "Description");

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryCheckConfigId",
                table: "InventoryChecks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckDate",
                table: "InventoryCheckConfigs",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CheckPeriod",
                table: "InventoryCheckConfigs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCheckedDate",
                table: "InventoryCheckConfigs",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryCheckConfigs",
                table: "InventoryCheckConfigs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryChecks_InventoryCheckConfigId",
                table: "InventoryChecks",
                column: "InventoryCheckConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryChecks_InventoryCheckConfigs_InventoryCheckConfigId",
                table: "InventoryChecks",
                column: "InventoryCheckConfigId",
                principalTable: "InventoryCheckConfigs",
                principalColumn: "Id");
        }
    }
}
