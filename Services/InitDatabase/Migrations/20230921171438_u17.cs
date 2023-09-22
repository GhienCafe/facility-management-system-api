using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Users_CreatorId",
                table: "Maintenances");

            migrationBuilder.DropTable(
                name: "MaintenanceDetails");

            migrationBuilder.DropTable(
                name: "ReplacementDetails");

            migrationBuilder.DropTable(
                name: "TransportationDetails");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_CreatorId",
                table: "Maintenances");

            migrationBuilder.RenameColumn(
                name: "IsSendAll",
                table: "Notifications",
                newName: "IsRead");

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetId",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId1",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ToRoomId",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetId",
                table: "Replacements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "NewAssetId",
                table: "Replacements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Replacements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemId",
                table: "MediaFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedTo",
                table: "MaintenanceScheduleConfigs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetId",
                table: "Maintenances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Maintenances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "AssetTypes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModelId",
                table: "Assets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Repairation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssignedTo = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repairation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repairation_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Repairation_Users_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Repairation_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TeamId",
                table: "Users",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Transportations_AssetId",
                table: "Transportations",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Transportations_CreatorId1",
                table: "Transportations",
                column: "CreatorId1");

            migrationBuilder.CreateIndex(
                name: "IX_Transportations_ToRoomId",
                table: "Transportations",
                column: "ToRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Replacements_AssetId",
                table: "Replacements",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Replacements_AssignedTo",
                table: "Replacements",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceScheduleConfigs_AssignedTo",
                table: "MaintenanceScheduleConfigs",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_AssetId",
                table: "Maintenances",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_AssignedTo",
                table: "Maintenances",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTypes_CategoryId",
                table: "AssetTypes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ModelId",
                table: "Assets",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TeamId",
                table: "Categories",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Repairation_AssetId",
                table: "Repairation",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Repairation_AssignedTo",
                table: "Repairation",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_Repairation_CreatorId",
                table: "Repairation",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Models_ModelId",
                table: "Assets",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTypes_Categories_CategoryId",
                table: "AssetTypes",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Assets_AssetId",
                table: "Maintenances",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Users_AssignedTo",
                table: "Maintenances",
                column: "AssignedTo",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceScheduleConfigs_Users_AssignedTo",
                table: "MaintenanceScheduleConfigs",
                column: "AssignedTo",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Replacements_Assets_AssetId",
                table: "Replacements",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Replacements_Users_AssignedTo",
                table: "Replacements",
                column: "AssignedTo",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transportations_Assets_AssetId",
                table: "Transportations",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transportations_Rooms_ToRoomId",
                table: "Transportations",
                column: "ToRoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transportations_Users_CreatorId1",
                table: "Transportations",
                column: "CreatorId1",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Teams_TeamId",
                table: "Users",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Models_ModelId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetTypes_Categories_CategoryId",
                table: "AssetTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Assets_AssetId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Users_AssignedTo",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceScheduleConfigs_Users_AssignedTo",
                table: "MaintenanceScheduleConfigs");

            migrationBuilder.DropForeignKey(
                name: "FK_Replacements_Assets_AssetId",
                table: "Replacements");

            migrationBuilder.DropForeignKey(
                name: "FK_Replacements_Users_AssignedTo",
                table: "Replacements");

            migrationBuilder.DropForeignKey(
                name: "FK_Transportations_Assets_AssetId",
                table: "Transportations");

            migrationBuilder.DropForeignKey(
                name: "FK_Transportations_Rooms_ToRoomId",
                table: "Transportations");

            migrationBuilder.DropForeignKey(
                name: "FK_Transportations_Users_CreatorId1",
                table: "Transportations");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Teams_TeamId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropTable(
                name: "Repairation");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Users_TeamId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Transportations_AssetId",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Transportations_CreatorId1",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Transportations_ToRoomId",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Replacements_AssetId",
                table: "Replacements");

            migrationBuilder.DropIndex(
                name: "IX_Replacements_AssignedTo",
                table: "Replacements");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceScheduleConfigs_AssignedTo",
                table: "MaintenanceScheduleConfigs");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_AssetId",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_AssignedTo",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_AssetTypes_CategoryId",
                table: "AssetTypes");

            migrationBuilder.DropIndex(
                name: "IX_Assets_ModelId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "CreatorId1",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "ToRoomId",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "NewAssetId",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "MaintenanceScheduleConfigs");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "AssetTypes");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "Notifications",
                newName: "IsSendAll");

            migrationBuilder.CreateTable(
                name: "MaintenanceDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaintenanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDone = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceDetails_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MaintenanceDetails_Maintenances_MaintenanceId",
                        column: x => x.MaintenanceId,
                        principalTable: "Maintenances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReplacementDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReplacementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDone = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacementAssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplacementDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReplacementDetails_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReplacementDetails_Replacements_ReplacementId",
                        column: x => x.ReplacementId,
                        principalTable: "Replacements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransportationDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransportationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DestinationLocation = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDone = table.Column<bool>(type: "bit", nullable: false),
                    SourceLocation = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportationDetails_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransportationDetails_Transportations_TransportationId",
                        column: x => x.TransportationId,
                        principalTable: "Transportations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_CreatorId",
                table: "Maintenances",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceDetails_AssetId",
                table: "MaintenanceDetails",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceDetails_MaintenanceId",
                table: "MaintenanceDetails",
                column: "MaintenanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplacementDetails_AssetId",
                table: "ReplacementDetails",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplacementDetails_ReplacementId",
                table: "ReplacementDetails",
                column: "ReplacementId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportationDetails_AssetId",
                table: "TransportationDetails",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportationDetails_TransportationId",
                table: "TransportationDetails",
                column: "TransportationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Users_CreatorId",
                table: "Maintenances",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
