using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u48 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "Key",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Maintenances");

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

            migrationBuilder.RenameColumn(
                name: "DeleterId",
                table: "MediaFiles",
                newName: "InventoryCheckId");

            migrationBuilder.AlterColumn<string>(
                name: "Extensions",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "InventoryCheckConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckPeriod = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryCheckConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventoryCheckConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsInternal = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    AssignedTo = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Checkin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Checkout = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryChecks_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryChecks_InventoryCheckConfigs_InventoryCheckConfigId",
                        column: x => x.InventoryCheckConfigId,
                        principalTable: "InventoryCheckConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryChecks_Users_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InventoryCheckDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventoryCheckId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryCheckDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryCheckDetails_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryCheckDetails_InventoryChecks_AssetId",
                        column: x => x.AssetId,
                        principalTable: "InventoryChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCheckDetails_AssetId",
                table: "InventoryCheckDetails",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryChecks_AssetId",
                table: "InventoryChecks",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryChecks_AssignedTo",
                table: "InventoryChecks",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryChecks_InventoryCheckConfigId",
                table: "InventoryChecks",
                column: "InventoryCheckConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_InventoryChecks_MaintenanceId",
                table: "MediaFiles",
                column: "MaintenanceId",
                principalTable: "InventoryChecks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_InventoryChecks_MaintenanceId",
                table: "MediaFiles");

            migrationBuilder.DropTable(
                name: "InventoryCheckDetails");

            migrationBuilder.DropTable(
                name: "InventoryChecks");

            migrationBuilder.DropTable(
                name: "InventoryCheckConfigs");

            migrationBuilder.RenameColumn(
                name: "InventoryCheckId",
                table: "MediaFiles",
                newName: "DeleterId");

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

            migrationBuilder.AlterColumn<string>(
                name: "Extensions",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "MediaFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Maintenances",
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
    }
}
