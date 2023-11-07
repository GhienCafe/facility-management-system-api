using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u52 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Repairations_RepairationId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Replacements_RepairationId",
                table: "MediaFiles");

            migrationBuilder.DropTable(
                name: "Repairations");

            migrationBuilder.RenameColumn(
                name: "RepairationId",
                table: "MediaFiles",
                newName: "RepairId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaFiles_RepairationId",
                table: "MediaFiles",
                newName: "IX_MediaFiles_RepairId");

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "InventoryChecks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Repairs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    Checkout = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repairs_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Repairs_Users_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryChecks_RoomId",
                table: "InventoryChecks",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_AssetId",
                table: "Repairs",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_AssignedTo",
                table: "Repairs",
                column: "AssignedTo");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryChecks_Rooms_RoomId",
                table: "InventoryChecks",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Repairs_RepairId",
                table: "MediaFiles",
                column: "RepairId",
                principalTable: "Repairs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Replacements_RepairId",
                table: "MediaFiles",
                column: "RepairId",
                principalTable: "Replacements",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryChecks_Rooms_RoomId",
                table: "InventoryChecks");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Repairs_RepairId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Replacements_RepairId",
                table: "MediaFiles");

            migrationBuilder.DropTable(
                name: "Repairs");

            migrationBuilder.DropIndex(
                name: "IX_InventoryChecks_RoomId",
                table: "InventoryChecks");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "InventoryChecks");

            migrationBuilder.RenameColumn(
                name: "RepairId",
                table: "MediaFiles",
                newName: "RepairationId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaFiles_RepairId",
                table: "MediaFiles",
                newName: "IX_MediaFiles_RepairationId");

            migrationBuilder.CreateTable(
                name: "Repairations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedTo = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssetTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Checkin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Checkout = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    Priority = table.Column<int>(type: "int", nullable: true),
                    RequestCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repairations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repairations_Assets_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Repairations_Users_AssignedTo",
                        column: x => x.AssignedTo,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Repairations_AssetId",
                table: "Repairations",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Repairations_AssignedTo",
                table: "Repairations",
                column: "AssignedTo");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Repairations_RepairationId",
                table: "MediaFiles",
                column: "RepairationId",
                principalTable: "Repairations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Replacements_RepairationId",
                table: "MediaFiles",
                column: "RepairationId",
                principalTable: "Replacements",
                principalColumn: "Id");
        }
    }
}
