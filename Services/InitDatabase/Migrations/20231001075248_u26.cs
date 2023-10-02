using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "FK_Repairation_Users_AssignedTo",
                table: "Repairation");

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
                name: "FK_Transportations_Users_AssignedTo",
                table: "Transportations");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Teams_TeamId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TeamId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Transportations_AssignedTo",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Replacements_AssignedTo",
                table: "Replacements");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceScheduleConfigs_AssignedTo",
                table: "MaintenanceScheduleConfigs");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_AssignedTo",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "CompletionDate",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "RequestCode",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "RequestedDate",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "CompletionDate",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "RequestCode",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "RequestedDate",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "CompletionDate",
                table: "Repairation");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Repairation");

            migrationBuilder.DropColumn(
                name: "RequestCode",
                table: "Repairation");

            migrationBuilder.DropColumn(
                name: "RequestedDate",
                table: "Repairation");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Repairation");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "MaintenanceScheduleConfigs");

            migrationBuilder.DropColumn(
                name: "SpecificDate",
                table: "MaintenanceScheduleConfigs");

            migrationBuilder.DropColumn(
                name: "TimeUnit",
                table: "MaintenanceScheduleConfigs");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "CompletionDate",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "RequestCode",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "RequestedDate",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Maintenances");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Repairation",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "AssignedTo",
                table: "Repairation",
                newName: "RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Repairation_AssignedTo",
                table: "Repairation",
                newName: "IX_Repairation_RequestId");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "Maintenances",
                newName: "Notes");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssetId",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RequestId",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "RoomAssets",
                type: "float",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AssetId",
                table: "Replacements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RequestId",
                table: "Replacements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "AssetId",
                table: "Maintenances",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RequestId",
                table: "Maintenances",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Floors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalArea",
                table: "Floors",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Campuses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "Campuses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Buildings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalArea",
                table: "Buildings",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCheckedDate",
                table: "Assets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDateOfUse",
                table: "Assets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestType = table.Column<int>(type: "int", nullable: true),
                    RequestStatus = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsInternal = table.Column<bool>(type: "bit", nullable: false),
                    AssignedTo = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Request_Users_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TeamMember",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsLead = table.Column<bool>(type: "bit", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMember_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMember_Users_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetCheck",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCheck", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetCheck_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetCheck_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transportations_RequestId",
                table: "Transportations",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Replacements_RequestId",
                table: "Replacements",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ItemId",
                table: "Notifications",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_RequestId",
                table: "Maintenances",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCheck_AssetId",
                table: "AssetCheck",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCheck_RequestId",
                table: "AssetCheck",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_AssignedTo",
                table: "Request",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_MemberId",
                table: "TeamMember",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_TeamId",
                table: "TeamMember",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Assets_AssetId",
                table: "Maintenances",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Request_RequestId",
                table: "Maintenances",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Request_ItemId",
                table: "Notifications",
                column: "ItemId",
                principalTable: "Request",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Repairation_Request_RequestId",
                table: "Repairation",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Replacements_Assets_AssetId",
                table: "Replacements",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Replacements_Request_RequestId",
                table: "Replacements",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transportations_Assets_AssetId",
                table: "Transportations",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transportations_Request_RequestId",
                table: "Transportations",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Assets_AssetId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Request_RequestId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Request_ItemId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Repairation_Request_RequestId",
                table: "Repairation");

            migrationBuilder.DropForeignKey(
                name: "FK_Replacements_Assets_AssetId",
                table: "Replacements");

            migrationBuilder.DropForeignKey(
                name: "FK_Replacements_Request_RequestId",
                table: "Replacements");

            migrationBuilder.DropForeignKey(
                name: "FK_Transportations_Assets_AssetId",
                table: "Transportations");

            migrationBuilder.DropForeignKey(
                name: "FK_Transportations_Request_RequestId",
                table: "Transportations");

            migrationBuilder.DropTable(
                name: "AssetCheck");

            migrationBuilder.DropTable(
                name: "TeamMember");

            migrationBuilder.DropTable(
                name: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Transportations_RequestId",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Replacements_RequestId",
                table: "Replacements");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ItemId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_RequestId",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "RoomAssets");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Floors");

            migrationBuilder.DropColumn(
                name: "TotalArea",
                table: "Floors");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Campuses");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "Campuses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "TotalArea",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "LastCheckedDate",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "StartDateOfUse",
                table: "Assets");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "Repairation",
                newName: "AssignedTo");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Repairation",
                newName: "Reason");

            migrationBuilder.RenameIndex(
                name: "IX_Repairation_RequestId",
                table: "Repairation",
                newName: "IX_Repairation_AssignedTo");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Maintenances",
                newName: "Note");

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AssetId",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedTo",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionDate",
                table: "Transportations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Transportations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Transportations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestCode",
                table: "Transportations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedDate",
                table: "Transportations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Transportations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "AssetId",
                table: "Replacements",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedTo",
                table: "Replacements",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionDate",
                table: "Replacements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Replacements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Replacements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Replacements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestCode",
                table: "Replacements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedDate",
                table: "Replacements",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Replacements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionDate",
                table: "Repairation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Repairation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestCode",
                table: "Repairation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedDate",
                table: "Repairation",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Repairation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedTo",
                table: "MaintenanceScheduleConfigs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SpecificDate",
                table: "MaintenanceScheduleConfigs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TimeUnit",
                table: "MaintenanceScheduleConfigs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "AssetId",
                table: "Maintenances",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedTo",
                table: "Maintenances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionDate",
                table: "Maintenances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Maintenances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestCode",
                table: "Maintenances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedDate",
                table: "Maintenances",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Maintenances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Maintenances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TeamId",
                table: "Users",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Transportations_AssignedTo",
                table: "Transportations",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_Replacements_AssignedTo",
                table: "Replacements",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceScheduleConfigs_AssignedTo",
                table: "MaintenanceScheduleConfigs",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_AssignedTo",
                table: "Maintenances",
                column: "AssignedTo");

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
                name: "FK_Repairation_Users_AssignedTo",
                table: "Repairation",
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
                name: "FK_Transportations_Users_AssignedTo",
                table: "Transportations",
                column: "AssignedTo",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Teams_TeamId",
                table: "Users",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }
    }
}
