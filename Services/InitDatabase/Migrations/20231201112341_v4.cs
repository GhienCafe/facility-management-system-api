using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReportId",
                table: "MediaFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsReject = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_Report", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Report_AssetChecks_AssetCheckId",
                        column: x => x.AssetCheckId,
                        principalTable: "AssetChecks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Report_InventoryChecks_InventoryCheckId",
                        column: x => x.InventoryCheckId,
                        principalTable: "InventoryChecks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Report_Maintenances_MaintenanceId",
                        column: x => x.MaintenanceId,
                        principalTable: "Maintenances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Report_Repairs_RepairId",
                        column: x => x.RepairId,
                        principalTable: "Repairs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Report_Replacements_ReplacementId",
                        column: x => x.ReplacementId,
                        principalTable: "Replacements",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Report_Transportations_TransportationId",
                        column: x => x.TransportationId,
                        principalTable: "Transportations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_ReportId",
                table: "MediaFiles",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_AssetCheckId",
                table: "Report",
                column: "AssetCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_InventoryCheckId",
                table: "Report",
                column: "InventoryCheckId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_MaintenanceId",
                table: "Report",
                column: "MaintenanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_RepairId",
                table: "Report",
                column: "RepairId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_ReplacementId",
                table: "Report",
                column: "ReplacementId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_TransportationId",
                table: "Report",
                column: "TransportationId");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Report_ReportId",
                table: "MediaFiles",
                column: "ReportId",
                principalTable: "Report",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Report_ReportId",
                table: "MediaFiles");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropIndex(
                name: "IX_MediaFiles_ReportId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "MediaFiles");
        }
    }
}
