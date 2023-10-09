using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u30 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetChecks_Requests_RequestId",
                table: "AssetChecks");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Requests_RequestId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Requests_ItemId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Repairations_Requests_RequestId",
                table: "Repairations");

            migrationBuilder.DropForeignKey(
                name: "FK_Replacements_Assets_AssetId",
                table: "Replacements");

            migrationBuilder.DropForeignKey(
                name: "FK_Replacements_Requests_RequestId",
                table: "Replacements");

            migrationBuilder.DropForeignKey(
                name: "FK_Transportations_Requests_RequestId",
                table: "Transportations");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Transportations_RequestId",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Replacements_AssetId",
                table: "Replacements");

            migrationBuilder.DropIndex(
                name: "IX_Replacements_RequestId",
                table: "Replacements");

            migrationBuilder.DropIndex(
                name: "IX_Repairations_RequestId",
                table: "Repairations");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ItemId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_RequestId",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_AssetChecks_RequestId",
                table: "AssetChecks");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Repairations");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "AssetChecks");

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

            migrationBuilder.AddColumn<bool>(
                name: "IsInternal",
                table: "Transportations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
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
                name: "RequestDate",
                table: "Transportations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Transportations",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "NewAssetId",
                table: "Replacements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

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

            migrationBuilder.AddColumn<bool>(
                name: "IsInternal",
                table: "Replacements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
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
                name: "RequestDate",
                table: "Replacements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Replacements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedTo",
                table: "Repairations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionDate",
                table: "Repairations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInternal",
                table: "Repairations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RequestCode",
                table: "Repairations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestDate",
                table: "Repairations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Repairations",
                type: "int",
                nullable: true);

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

            migrationBuilder.AddColumn<bool>(
                name: "IsInternal",
                table: "Maintenances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RequestCode",    
                table: "Maintenances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestDate",
                table: "Maintenances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Maintenances",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsVerified",
                table: "AssetChecks",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedTo",
                table: "AssetChecks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionDate",
                table: "AssetChecks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AssetChecks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInternal",
                table: "AssetChecks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "AssetChecks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestCode",
                table: "AssetChecks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestDate",
                table: "AssetChecks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "AssetChecks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transportations_AssignedTo",
                table: "Transportations",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_Replacements_AssignedTo",
                table: "Replacements",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_Replacements_NewAssetId",
                table: "Replacements",
                column: "NewAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Repairations_AssignedTo",
                table: "Repairations",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_AssignedTo",
                table: "Maintenances",
                column: "AssignedTo");

            migrationBuilder.CreateIndex(
                name: "IX_AssetChecks_AssignedTo",
                table: "AssetChecks",
                column: "AssignedTo");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetChecks_Users_AssignedTo",
                table: "AssetChecks",
                column: "AssignedTo",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Users_AssignedTo",
                table: "Maintenances",
                column: "AssignedTo",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Repairations_Users_AssignedTo",
                table: "Repairations",
                column: "AssignedTo",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Replacements_Assets_NewAssetId",
                table: "Replacements",
                column: "NewAssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Replacements_Users_AssignedTo",
                table: "Replacements",
                column: "AssignedTo",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transportations_Users_AssignedTo",
                table: "Transportations",
                column: "AssignedTo",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetChecks_Users_AssignedTo",
                table: "AssetChecks");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Users_AssignedTo",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Repairations_Users_AssignedTo",
                table: "Repairations");

            migrationBuilder.DropForeignKey(
                name: "FK_Replacements_Assets_NewAssetId",
                table: "Replacements");

            migrationBuilder.DropForeignKey(
                name: "FK_Replacements_Users_AssignedTo",
                table: "Replacements");

            migrationBuilder.DropForeignKey(
                name: "FK_Transportations_Users_AssignedTo",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Transportations_AssignedTo",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Replacements_AssignedTo",
                table: "Replacements");

            migrationBuilder.DropIndex(
                name: "IX_Replacements_NewAssetId",
                table: "Replacements");

            migrationBuilder.DropIndex(
                name: "IX_Repairations_AssignedTo",
                table: "Repairations");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_AssignedTo",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_AssetChecks_AssignedTo",
                table: "AssetChecks");

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
                name: "IsInternal",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "RequestCode",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "RequestDate",
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
                name: "IsInternal",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "RequestCode",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "Repairations");

            migrationBuilder.DropColumn(
                name: "CompletionDate",
                table: "Repairations");

            migrationBuilder.DropColumn(
                name: "IsInternal",
                table: "Repairations");

            migrationBuilder.DropColumn(
                name: "RequestCode",
                table: "Repairations");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "Repairations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Repairations");

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
                name: "IsInternal",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "RequestCode",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "AssetChecks");

            migrationBuilder.DropColumn(
                name: "CompletionDate",
                table: "AssetChecks");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AssetChecks");

            migrationBuilder.DropColumn(
                name: "IsInternal",
                table: "AssetChecks");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "AssetChecks");

            migrationBuilder.DropColumn(
                name: "RequestCode",
                table: "AssetChecks");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "AssetChecks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AssetChecks");

            migrationBuilder.AddColumn<Guid>(
                name: "RequestId",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "NewAssetId",
                table: "Replacements",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "RequestId",
                table: "Replacements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RequestId",
                table: "Repairations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RequestId",
                table: "Maintenances",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<bool>(
                name: "IsVerified",
                table: "AssetChecks",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "RequestId",
                table: "AssetChecks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedTo = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsInternal = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestStatus = table.Column<int>(type: "int", nullable: true),
                    RequestType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_Users_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transportations_RequestId",
                table: "Transportations",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Replacements_AssetId",
                table: "Replacements",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Replacements_RequestId",
                table: "Replacements",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Repairations_RequestId",
                table: "Repairations",
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
                name: "IX_AssetChecks_RequestId",
                table: "AssetChecks",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_AssignedTo",
                table: "Requests",
                column: "AssignedTo");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetChecks_Requests_RequestId",
                table: "AssetChecks",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Requests_RequestId",
                table: "Maintenances",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Requests_ItemId",
                table: "Notifications",
                column: "ItemId",
                principalTable: "Requests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Repairations_Requests_RequestId",
                table: "Repairations",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Replacements_Assets_AssetId",
                table: "Replacements",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Replacements_Requests_RequestId",
                table: "Replacements",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transportations_Requests_RequestId",
                table: "Transportations",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
