using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Reports_ReportId",
                table: "MediaFiles");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_ReportId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "MediaFiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReportId",
                table: "MediaFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetCheckId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InventoryCheckId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MaintenanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RepairId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReplacementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransportationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsReject = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RejectReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportContent = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                        name: "FK_Reports_Replacements_ReplacementId",
                        column: x => x.ReplacementId,
                        principalTable: "Replacements",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reports_Transportations_TransportationId",
                        column: x => x.TransportationId,
                        principalTable: "Transportations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_ReportId",
                table: "MediaFiles",
                column: "ReportId");

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
                name: "IX_Reports_ReplacementId",
                table: "Reports",
                column: "ReplacementId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_TransportationId",
                table: "Reports",
                column: "TransportationId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Reports_ReportId",
                table: "MediaFiles",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "Id");
        }
    }
}
