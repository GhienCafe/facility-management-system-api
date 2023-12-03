using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class v7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaFiles");

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsReported = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsReject = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RejectReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaintenanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReplacementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssetCheckId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RepairId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransportationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InventoryCheckId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_AssetChecks_AssetCheckId",
                        column: x => x.AssetCheckId,
                        principalTable: "AssetChecks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_InventoryChecks_InventoryCheckId",
                        column: x => x.InventoryCheckId,
                        principalTable: "InventoryChecks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Maintenances_MaintenanceId",
                        column: x => x.MaintenanceId,
                        principalTable: "Maintenances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Repairs_RepairId",
                        column: x => x.RepairId,
                        principalTable: "Repairs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Replacements_RepairId",
                        column: x => x.RepairId,
                        principalTable: "Replacements",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Transportations_TransportationId",
                        column: x => x.TransportationId,
                        principalTable: "Transportations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_AssetCheckId",
                table: "Reports",
                column: "AssetCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_InventoryCheckId",
                table: "Reports",
                column: "InventoryCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_MaintenanceId",
                table: "Reports",
                column: "MaintenanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_RepairId",
                table: "Reports",
                column: "RepairId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_TransportationId",
                table: "Reports",
                column: "TransportationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.CreateTable(
                name: "MediaFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetCheckId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InventoryCheckId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaintenanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RepairId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransportationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileType = table.Column<int>(type: "int", nullable: false),
                    IsReported = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RawUri = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Uri = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaFiles_AssetChecks_AssetCheckId",
                        column: x => x.AssetCheckId,
                        principalTable: "AssetChecks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MediaFiles_InventoryChecks_InventoryCheckId",
                        column: x => x.InventoryCheckId,
                        principalTable: "InventoryChecks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MediaFiles_Maintenances_MaintenanceId",
                        column: x => x.MaintenanceId,
                        principalTable: "Maintenances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MediaFiles_Repairs_RepairId",
                        column: x => x.RepairId,
                        principalTable: "Repairs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MediaFiles_Replacements_RepairId",
                        column: x => x.RepairId,
                        principalTable: "Replacements",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MediaFiles_Transportations_TransportationId",
                        column: x => x.TransportationId,
                        principalTable: "Transportations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_AssetCheckId",
                table: "MediaFiles",
                column: "AssetCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_InventoryCheckId",
                table: "MediaFiles",
                column: "InventoryCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_MaintenanceId",
                table: "MediaFiles",
                column: "MaintenanceId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_RepairId",
                table: "MediaFiles",
                column: "RepairId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_TransportationId",
                table: "MediaFiles",
                column: "TransportationId");
        }
    }
}
