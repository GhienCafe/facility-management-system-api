using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HandoverDetail_AssetHandover_AssetHandoverId",
                table: "HandoverDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_HandoverDetail_Assets_AssetCode",
                table: "HandoverDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_HandoverParticipant_AssetHandover_AssetHandoverId",
                table: "HandoverParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_HandoverParticipant_Users_UserId",
                table: "HandoverParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventory_Campuses_CampusId",
                table: "Inventory");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryDetail_Assets_AssetCode",
                table: "InventoryDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryDetail_Inventory_InventoryId",
                table: "InventoryDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTeam_Inventory_InventoryId",
                table: "InventoryTeam");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTeamMember_InventoryTeam_InventoryTeamId",
                table: "InventoryTeamMember");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTeamMember_Users_UserId",
                table: "InventoryTeamMember");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceDetail_Assets_AssetCode",
                table: "MaintenanceDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceDetail_Maintenance_MaintenanceId",
                table: "MaintenanceDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceParticipant_Maintenance_MaintenanceId",
                table: "MaintenanceParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceParticipant_Users_UserId",
                table: "MaintenanceParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceParticipant",
                table: "MaintenanceParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceDetail",
                table: "MaintenanceDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Maintenance",
                table: "Maintenance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryTeamMember",
                table: "InventoryTeamMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryTeam",
                table: "InventoryTeam");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryDetail",
                table: "InventoryDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventory",
                table: "Inventory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HandoverParticipant",
                table: "HandoverParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HandoverDetail",
                table: "HandoverDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetHandover",
                table: "AssetHandover");

            migrationBuilder.DropColumn(
                name: "HandoverCode",
                table: "HandoverDetail");

            migrationBuilder.RenameTable(
                name: "MaintenanceParticipant",
                newName: "MaintenanceParticipants");

            migrationBuilder.RenameTable(
                name: "MaintenanceDetail",
                newName: "MaintenanceDetails");

            migrationBuilder.RenameTable(
                name: "Maintenance",
                newName: "Maintenances");

            migrationBuilder.RenameTable(
                name: "InventoryTeamMember",
                newName: "InventoryTeamMembers");

            migrationBuilder.RenameTable(
                name: "InventoryTeam",
                newName: "InventoryTeams");

            migrationBuilder.RenameTable(
                name: "InventoryDetail",
                newName: "InventoryDetails");

            migrationBuilder.RenameTable(
                name: "Inventory",
                newName: "Inventories");

            migrationBuilder.RenameTable(
                name: "HandoverParticipant",
                newName: "HandoverParticipants");

            migrationBuilder.RenameTable(
                name: "HandoverDetail",
                newName: "HandoverDetails");

            migrationBuilder.RenameTable(
                name: "AssetHandover",
                newName: "AssetHandovers");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceParticipant_UserId",
                table: "MaintenanceParticipants",
                newName: "IX_MaintenanceParticipants_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceParticipant_MaintenanceId",
                table: "MaintenanceParticipants",
                newName: "IX_MaintenanceParticipants_MaintenanceId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceDetail_MaintenanceId",
                table: "MaintenanceDetails",
                newName: "IX_MaintenanceDetails_MaintenanceId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceDetail_AssetCode",
                table: "MaintenanceDetails",
                newName: "IX_MaintenanceDetails_AssetCode");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryTeamMember_UserId",
                table: "InventoryTeamMembers",
                newName: "IX_InventoryTeamMembers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryTeamMember_InventoryTeamId",
                table: "InventoryTeamMembers",
                newName: "IX_InventoryTeamMembers_InventoryTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryTeam_InventoryId",
                table: "InventoryTeams",
                newName: "IX_InventoryTeams_InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryDetail_InventoryId",
                table: "InventoryDetails",
                newName: "IX_InventoryDetails_InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryDetail_AssetCode",
                table: "InventoryDetails",
                newName: "IX_InventoryDetails_AssetCode");

            migrationBuilder.RenameIndex(
                name: "IX_Inventory_CampusId",
                table: "Inventories",
                newName: "IX_Inventories_CampusId");

            migrationBuilder.RenameIndex(
                name: "IX_HandoverParticipant_UserId",
                table: "HandoverParticipants",
                newName: "IX_HandoverParticipants_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_HandoverParticipant_AssetHandoverId",
                table: "HandoverParticipants",
                newName: "IX_HandoverParticipants_AssetHandoverId");

            migrationBuilder.RenameIndex(
                name: "IX_HandoverDetail_AssetHandoverId",
                table: "HandoverDetails",
                newName: "IX_HandoverDetails_AssetHandoverId");

            migrationBuilder.RenameIndex(
                name: "IX_HandoverDetail_AssetCode",
                table: "HandoverDetails",
                newName: "IX_HandoverDetails_AssetCode");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "RoomStatus",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InventoryCode",
                table: "Inventories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AssetHandoverId",
                table: "HandoverDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HandOverCode",
                table: "AssetHandovers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceParticipants",
                table: "MaintenanceParticipants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceDetails",
                table: "MaintenanceDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Maintenances",
                table: "Maintenances",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryTeamMembers",
                table: "InventoryTeamMembers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryTeams",
                table: "InventoryTeams",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryDetails",
                table: "InventoryDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HandoverParticipants",
                table: "HandoverParticipants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HandoverDetails",
                table: "HandoverDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetHandovers",
                table: "AssetHandovers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "MediaFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RawUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Extensions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_InventoryCode",
                table: "Inventories",
                column: "InventoryCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetHandovers_HandOverCode",
                table: "AssetHandovers",
                column: "HandOverCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HandoverDetails_AssetHandovers_AssetHandoverId",
                table: "HandoverDetails",
                column: "AssetHandoverId",
                principalTable: "AssetHandovers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HandoverDetails_Assets_AssetCode",
                table: "HandoverDetails",
                column: "AssetCode",
                principalTable: "Assets",
                principalColumn: "AssetCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HandoverParticipants_AssetHandovers_AssetHandoverId",
                table: "HandoverParticipants",
                column: "AssetHandoverId",
                principalTable: "AssetHandovers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HandoverParticipants_Users_UserId",
                table: "HandoverParticipants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Campuses_CampusId",
                table: "Inventories",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryDetails_Assets_AssetCode",
                table: "InventoryDetails",
                column: "AssetCode",
                principalTable: "Assets",
                principalColumn: "AssetCode");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryDetails_Inventories_InventoryId",
                table: "InventoryDetails",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTeamMembers_InventoryTeams_InventoryTeamId",
                table: "InventoryTeamMembers",
                column: "InventoryTeamId",
                principalTable: "InventoryTeams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTeamMembers_Users_UserId",
                table: "InventoryTeamMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTeams_Inventories_InventoryId",
                table: "InventoryTeams",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceDetails_Assets_AssetCode",
                table: "MaintenanceDetails",
                column: "AssetCode",
                principalTable: "Assets",
                principalColumn: "AssetCode");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceDetails_Maintenances_MaintenanceId",
                table: "MaintenanceDetails",
                column: "MaintenanceId",
                principalTable: "Maintenances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceParticipants_Maintenances_MaintenanceId",
                table: "MaintenanceParticipants",
                column: "MaintenanceId",
                principalTable: "Maintenances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceParticipants_Users_UserId",
                table: "MaintenanceParticipants",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HandoverDetails_AssetHandovers_AssetHandoverId",
                table: "HandoverDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_HandoverDetails_Assets_AssetCode",
                table: "HandoverDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_HandoverParticipants_AssetHandovers_AssetHandoverId",
                table: "HandoverParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_HandoverParticipants_Users_UserId",
                table: "HandoverParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Campuses_CampusId",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryDetails_Assets_AssetCode",
                table: "InventoryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryDetails_Inventories_InventoryId",
                table: "InventoryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTeamMembers_InventoryTeams_InventoryTeamId",
                table: "InventoryTeamMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTeamMembers_Users_UserId",
                table: "InventoryTeamMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTeams_Inventories_InventoryId",
                table: "InventoryTeams");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceDetails_Assets_AssetCode",
                table: "MaintenanceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceDetails_Maintenances_MaintenanceId",
                table: "MaintenanceDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceParticipants_Maintenances_MaintenanceId",
                table: "MaintenanceParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceParticipants_Users_UserId",
                table: "MaintenanceParticipants");

            migrationBuilder.DropTable(
                name: "MediaFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Maintenances",
                table: "Maintenances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceParticipants",
                table: "MaintenanceParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaintenanceDetails",
                table: "MaintenanceDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryTeams",
                table: "InventoryTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryTeamMembers",
                table: "InventoryTeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InventoryDetails",
                table: "InventoryDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_InventoryCode",
                table: "Inventories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HandoverParticipants",
                table: "HandoverParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HandoverDetails",
                table: "HandoverDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetHandovers",
                table: "AssetHandovers");

            migrationBuilder.DropIndex(
                name: "IX_AssetHandovers_HandOverCode",
                table: "AssetHandovers");

            migrationBuilder.RenameTable(
                name: "Maintenances",
                newName: "Maintenance");

            migrationBuilder.RenameTable(
                name: "MaintenanceParticipants",
                newName: "MaintenanceParticipant");

            migrationBuilder.RenameTable(
                name: "MaintenanceDetails",
                newName: "MaintenanceDetail");

            migrationBuilder.RenameTable(
                name: "InventoryTeams",
                newName: "InventoryTeam");

            migrationBuilder.RenameTable(
                name: "InventoryTeamMembers",
                newName: "InventoryTeamMember");

            migrationBuilder.RenameTable(
                name: "InventoryDetails",
                newName: "InventoryDetail");

            migrationBuilder.RenameTable(
                name: "Inventories",
                newName: "Inventory");

            migrationBuilder.RenameTable(
                name: "HandoverParticipants",
                newName: "HandoverParticipant");

            migrationBuilder.RenameTable(
                name: "HandoverDetails",
                newName: "HandoverDetail");

            migrationBuilder.RenameTable(
                name: "AssetHandovers",
                newName: "AssetHandover");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceParticipants_UserId",
                table: "MaintenanceParticipant",
                newName: "IX_MaintenanceParticipant_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceParticipants_MaintenanceId",
                table: "MaintenanceParticipant",
                newName: "IX_MaintenanceParticipant_MaintenanceId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceDetails_MaintenanceId",
                table: "MaintenanceDetail",
                newName: "IX_MaintenanceDetail_MaintenanceId");

            migrationBuilder.RenameIndex(
                name: "IX_MaintenanceDetails_AssetCode",
                table: "MaintenanceDetail",
                newName: "IX_MaintenanceDetail_AssetCode");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryTeams_InventoryId",
                table: "InventoryTeam",
                newName: "IX_InventoryTeam_InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryTeamMembers_UserId",
                table: "InventoryTeamMember",
                newName: "IX_InventoryTeamMember_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryTeamMembers_InventoryTeamId",
                table: "InventoryTeamMember",
                newName: "IX_InventoryTeamMember_InventoryTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryDetails_InventoryId",
                table: "InventoryDetail",
                newName: "IX_InventoryDetail_InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryDetails_AssetCode",
                table: "InventoryDetail",
                newName: "IX_InventoryDetail_AssetCode");

            migrationBuilder.RenameIndex(
                name: "IX_Inventories_CampusId",
                table: "Inventory",
                newName: "IX_Inventory_CampusId");

            migrationBuilder.RenameIndex(
                name: "IX_HandoverParticipants_UserId",
                table: "HandoverParticipant",
                newName: "IX_HandoverParticipant_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_HandoverParticipants_AssetHandoverId",
                table: "HandoverParticipant",
                newName: "IX_HandoverParticipant_AssetHandoverId");

            migrationBuilder.RenameIndex(
                name: "IX_HandoverDetails_AssetHandoverId",
                table: "HandoverDetail",
                newName: "IX_HandoverDetail_AssetHandoverId");

            migrationBuilder.RenameIndex(
                name: "IX_HandoverDetails_AssetCode",
                table: "HandoverDetail",
                newName: "IX_HandoverDetail_AssetCode");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "RoomStatus",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "InventoryCode",
                table: "Inventory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssetHandoverId",
                table: "HandoverDetail",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "HandoverCode",
                table: "HandoverDetail",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "HandOverCode",
                table: "AssetHandover",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Maintenance",
                table: "Maintenance",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceParticipant",
                table: "MaintenanceParticipant",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaintenanceDetail",
                table: "MaintenanceDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryTeam",
                table: "InventoryTeam",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryTeamMember",
                table: "InventoryTeamMember",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InventoryDetail",
                table: "InventoryDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventory",
                table: "Inventory",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HandoverParticipant",
                table: "HandoverParticipant",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HandoverDetail",
                table: "HandoverDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetHandover",
                table: "AssetHandover",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HandoverDetail_AssetHandover_AssetHandoverId",
                table: "HandoverDetail",
                column: "AssetHandoverId",
                principalTable: "AssetHandover",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HandoverDetail_Assets_AssetCode",
                table: "HandoverDetail",
                column: "AssetCode",
                principalTable: "Assets",
                principalColumn: "AssetCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HandoverParticipant_AssetHandover_AssetHandoverId",
                table: "HandoverParticipant",
                column: "AssetHandoverId",
                principalTable: "AssetHandover",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HandoverParticipant_Users_UserId",
                table: "HandoverParticipant",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventory_Campuses_CampusId",
                table: "Inventory",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryDetail_Assets_AssetCode",
                table: "InventoryDetail",
                column: "AssetCode",
                principalTable: "Assets",
                principalColumn: "AssetCode");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryDetail_Inventory_InventoryId",
                table: "InventoryDetail",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTeam_Inventory_InventoryId",
                table: "InventoryTeam",
                column: "InventoryId",
                principalTable: "Inventory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTeamMember_InventoryTeam_InventoryTeamId",
                table: "InventoryTeamMember",
                column: "InventoryTeamId",
                principalTable: "InventoryTeam",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTeamMember_Users_UserId",
                table: "InventoryTeamMember",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceDetail_Assets_AssetCode",
                table: "MaintenanceDetail",
                column: "AssetCode",
                principalTable: "Assets",
                principalColumn: "AssetCode");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceDetail_Maintenance_MaintenanceId",
                table: "MaintenanceDetail",
                column: "MaintenanceId",
                principalTable: "Maintenance",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceParticipant_Maintenance_MaintenanceId",
                table: "MaintenanceParticipant",
                column: "MaintenanceId",
                principalTable: "Maintenance",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceParticipant_Users_UserId",
                table: "MaintenanceParticipant",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
