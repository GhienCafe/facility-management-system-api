using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_AssetCategory_AssetCategoryId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_Campus_CampusId",
                table: "Buildings");

            migrationBuilder.DropForeignKey(
                name: "FK_Department_Campus_CampusId",
                table: "Department");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestDetail_AssetCategory_CategoryId",
                table: "RequestDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestDetail_AssetRequest_AssetRequestId",
                table: "RequestDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestParticipant_AssetRequest_AssetRequestId",
                table: "RequestParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestParticipant_Users_UserId",
                table: "RequestParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomAsset_Asset_AssetId",
                table: "RoomAsset");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomAsset_Rooms_RoomId",
                table: "RoomAsset");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_ColorStatus_ColorStatusId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Department_DepartmentId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "ColorStatus");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_ColorStatusId",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomAsset",
                table: "RoomAsset");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestParticipant",
                table: "RequestParticipant");

            migrationBuilder.DropIndex(
                name: "IX_RequestParticipant_AssetRequestId",
                table: "RequestParticipant");

            migrationBuilder.DropIndex(
                name: "IX_RequestParticipant_UserId",
                table: "RequestParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestDetail",
                table: "RequestDetail");

            migrationBuilder.DropIndex(
                name: "IX_RequestDetail_AssetRequestId",
                table: "RequestDetail");

            migrationBuilder.DropIndex(
                name: "IX_RequestDetail_CategoryId",
                table: "RequestDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Department",
                table: "Department");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Campus",
                table: "Campus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetRequest",
                table: "AssetRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetCategory",
                table: "AssetCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Asset",
                table: "Asset");

            migrationBuilder.DropIndex(
                name: "IX_Asset_AssetCategoryId",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "RoomNumber",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "AssetRequestId",
                table: "RequestParticipant");

            migrationBuilder.DropColumn(
                name: "AssetRequestId",
                table: "RequestDetail");

            migrationBuilder.DropColumn(
                name: "AssetCategoryId",
                table: "Asset");

            migrationBuilder.RenameTable(
                name: "RoomAsset",
                newName: "RoomAssets");

            migrationBuilder.RenameTable(
                name: "RequestParticipant",
                newName: "RequestParticipants");

            migrationBuilder.RenameTable(
                name: "RequestDetail",
                newName: "RequestDetails");

            migrationBuilder.RenameTable(
                name: "Department",
                newName: "Departments");

            migrationBuilder.RenameTable(
                name: "Campus",
                newName: "Campuses");

            migrationBuilder.RenameTable(
                name: "AssetRequest",
                newName: "AssetRequests");

            migrationBuilder.RenameTable(
                name: "AssetCategory",
                newName: "AssetCategories");

            migrationBuilder.RenameTable(
                name: "Asset",
                newName: "Assets");

            migrationBuilder.RenameColumn(
                name: "ColorStatusId",
                table: "Rooms",
                newName: "StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomAsset_RoomId",
                table: "RoomAssets",
                newName: "IX_RoomAssets_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomAsset_AssetId",
                table: "RoomAssets",
                newName: "IX_RoomAssets_AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_Department_CampusId",
                table: "Departments",
                newName: "IX_Departments_CampusId");

            migrationBuilder.AddColumn<string>(
                name: "RoomCode",
                table: "Rooms",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentCode",
                table: "Departments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RequestCode",
                table: "AssetRequests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryCode",
                table: "AssetCategories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastMaintenanceTime",
                table: "Assets",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "AssetCode",
                table: "Assets",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomAssets",
                table: "RoomAssets",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestParticipants",
                table: "RequestParticipants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestDetails",
                table: "RequestDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departments",
                table: "Departments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Campuses",
                table: "Campuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetRequests",
                table: "AssetRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetCategories",
                table: "AssetCategories",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Assets_AssetCode",
                table: "Assets",
                column: "AssetCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Assets",
                table: "Assets",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AssetHandover",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HandOverCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetHandover", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HandoverDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HandoverCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AssetCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    AssetHandoverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandoverDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HandoverDetail_AssetHandover_AssetHandoverId",
                        column: x => x.AssetHandoverId,
                        principalTable: "AssetHandover",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HandoverDetail_Assets_AssetCode",
                        column: x => x.AssetCode,
                        principalTable: "Assets",
                        principalColumn: "AssetCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HandoverParticipant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetHandoverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandoverParticipant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HandoverParticipant_AssetHandover_AssetHandoverId",
                        column: x => x.AssetHandoverId,
                        principalTable: "AssetHandover",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HandoverParticipant_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomCode",
                table: "Rooms",
                column: "RoomCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestParticipants_RequestId",
                table: "RequestParticipants",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestDetails_RequestId",
                table: "RequestDetails",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentCode",
                table: "Departments",
                column: "DepartmentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetRequests_RequestCode",
                table: "AssetRequests",
                column: "RequestCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetCategories_CategoryCode",
                table: "AssetCategories",
                column: "CategoryCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetCode",
                table: "Assets",
                column: "AssetCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CategoryId",
                table: "Assets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_HandoverDetail_AssetCode",
                table: "HandoverDetail",
                column: "AssetCode");

            migrationBuilder.CreateIndex(
                name: "IX_HandoverDetail_AssetHandoverId",
                table: "HandoverDetail",
                column: "AssetHandoverId");

            migrationBuilder.CreateIndex(
                name: "IX_HandoverParticipant_AssetHandoverId",
                table: "HandoverParticipant",
                column: "AssetHandoverId");

            migrationBuilder.CreateIndex(
                name: "IX_HandoverParticipant_UserId",
                table: "HandoverParticipant",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetCategories_CategoryId",
                table: "Assets",
                column: "CategoryId",
                principalTable: "AssetCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_Campuses_CampusId",
                table: "Buildings",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Campuses_CampusId",
                table: "Departments",
                column: "CampusId",
                principalTable: "Campuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestDetails_AssetCategories_RequestId",
                table: "RequestDetails",
                column: "RequestId",
                principalTable: "AssetCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestDetails_AssetRequests_RequestId",
                table: "RequestDetails",
                column: "RequestId",
                principalTable: "AssetRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestParticipants_AssetRequests_RequestId",
                table: "RequestParticipants",
                column: "RequestId",
                principalTable: "AssetRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestParticipants_Users_RequestId",
                table: "RequestParticipants",
                column: "RequestId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAssets_Assets_AssetId",
                table: "RoomAssets",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAssets_Rooms_RoomId",
                table: "RoomAssets",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Departments_DepartmentId",
                table: "Users",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetCategories_CategoryId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_Campuses_CampusId",
                table: "Buildings");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Campuses_CampusId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestDetails_AssetCategories_RequestId",
                table: "RequestDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestDetails_AssetRequests_RequestId",
                table: "RequestDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestParticipants_AssetRequests_RequestId",
                table: "RequestParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestParticipants_Users_RequestId",
                table: "RequestParticipants");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomAssets_Assets_AssetId",
                table: "RoomAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomAssets_Rooms_RoomId",
                table: "RoomAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Departments_DepartmentId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "HandoverDetail");

            migrationBuilder.DropTable(
                name: "HandoverParticipant");

            migrationBuilder.DropTable(
                name: "AssetHandover");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_RoomCode",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomAssets",
                table: "RoomAssets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestParticipants",
                table: "RequestParticipants");

            migrationBuilder.DropIndex(
                name: "IX_RequestParticipants_RequestId",
                table: "RequestParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestDetails",
                table: "RequestDetails");

            migrationBuilder.DropIndex(
                name: "IX_RequestDetails_RequestId",
                table: "RequestDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departments",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_DepartmentCode",
                table: "Departments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Campuses",
                table: "Campuses");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Assets_AssetCode",
                table: "Assets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Assets",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_AssetCode",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_CategoryId",
                table: "Assets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetRequests",
                table: "AssetRequests");

            migrationBuilder.DropIndex(
                name: "IX_AssetRequests_RequestCode",
                table: "AssetRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetCategories",
                table: "AssetCategories");

            migrationBuilder.DropIndex(
                name: "IX_AssetCategories_CategoryCode",
                table: "AssetCategories");

            migrationBuilder.DropColumn(
                name: "RoomCode",
                table: "Rooms");

            migrationBuilder.RenameTable(
                name: "RoomAssets",
                newName: "RoomAsset");

            migrationBuilder.RenameTable(
                name: "RequestParticipants",
                newName: "RequestParticipant");

            migrationBuilder.RenameTable(
                name: "RequestDetails",
                newName: "RequestDetail");

            migrationBuilder.RenameTable(
                name: "Departments",
                newName: "Department");

            migrationBuilder.RenameTable(
                name: "Campuses",
                newName: "Campus");

            migrationBuilder.RenameTable(
                name: "Assets",
                newName: "Asset");

            migrationBuilder.RenameTable(
                name: "AssetRequests",
                newName: "AssetRequest");

            migrationBuilder.RenameTable(
                name: "AssetCategories",
                newName: "AssetCategory");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Rooms",
                newName: "ColorStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomAssets_RoomId",
                table: "RoomAsset",
                newName: "IX_RoomAsset_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomAssets_AssetId",
                table: "RoomAsset",
                newName: "IX_RoomAsset_AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_CampusId",
                table: "Department",
                newName: "IX_Department_CampusId");

            migrationBuilder.AddColumn<string>(
                name: "RoomNumber",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetRequestId",
                table: "RequestParticipant",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetRequestId",
                table: "RequestDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentCode",
                table: "Department",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastMaintenanceTime",
                table: "Asset",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AssetCode",
                table: "Asset",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "AssetCategoryId",
                table: "Asset",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RequestCode",
                table: "AssetRequest",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryCode",
                table: "AssetCategory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomAsset",
                table: "RoomAsset",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestParticipant",
                table: "RequestParticipant",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestDetail",
                table: "RequestDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Department",
                table: "Department",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Campus",
                table: "Campus",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Asset",
                table: "Asset",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetRequest",
                table: "AssetRequest",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetCategory",
                table: "AssetCategory",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ColorStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_ColorStatusId",
                table: "Rooms",
                column: "ColorStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestParticipant_AssetRequestId",
                table: "RequestParticipant",
                column: "AssetRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestParticipant_UserId",
                table: "RequestParticipant",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestDetail_AssetRequestId",
                table: "RequestDetail",
                column: "AssetRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestDetail_CategoryId",
                table: "RequestDetail",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_AssetCategoryId",
                table: "Asset",
                column: "AssetCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_AssetCategory_AssetCategoryId",
                table: "Asset",
                column: "AssetCategoryId",
                principalTable: "AssetCategory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_Campus_CampusId",
                table: "Buildings",
                column: "CampusId",
                principalTable: "Campus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Department_Campus_CampusId",
                table: "Department",
                column: "CampusId",
                principalTable: "Campus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestDetail_AssetCategory_CategoryId",
                table: "RequestDetail",
                column: "CategoryId",
                principalTable: "AssetCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestDetail_AssetRequest_AssetRequestId",
                table: "RequestDetail",
                column: "AssetRequestId",
                principalTable: "AssetRequest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestParticipant_AssetRequest_AssetRequestId",
                table: "RequestParticipant",
                column: "AssetRequestId",
                principalTable: "AssetRequest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestParticipant_Users_UserId",
                table: "RequestParticipant",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAsset_Asset_AssetId",
                table: "RoomAsset",
                column: "AssetId",
                principalTable: "Asset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAsset_Rooms_RoomId",
                table: "RoomAsset",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_ColorStatus_ColorStatusId",
                table: "Rooms",
                column: "ColorStatusId",
                principalTable: "ColorStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Department_DepartmentId",
                table: "Users",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
