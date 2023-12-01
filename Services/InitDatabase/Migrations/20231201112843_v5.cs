using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Report_ReportId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_AssetChecks_AssetCheckId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_InventoryChecks_InventoryCheckId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Maintenances_MaintenanceId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Repairs_RepairId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Replacements_ReplacementId",
                table: "Report");

            migrationBuilder.DropForeignKey(
                name: "FK_Report_Transportations_TransportationId",
                table: "Report");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Report",
                table: "Report");

            migrationBuilder.RenameTable(
                name: "Report",
                newName: "Reports");

            migrationBuilder.RenameIndex(
                name: "IX_Report_TransportationId",
                table: "Reports",
                newName: "IX_Reports_TransportationId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_ReplacementId",
                table: "Reports",
                newName: "IX_Reports_ReplacementId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_RepairId",
                table: "Reports",
                newName: "IX_Reports_RepairId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_MaintenanceId",
                table: "Reports",
                newName: "IX_Reports_MaintenanceId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_InventoryCheckId",
                table: "Reports",
                newName: "IX_Reports_InventoryCheckId");

            migrationBuilder.RenameIndex(
                name: "IX_Report_AssetCheckId",
                table: "Reports",
                newName: "IX_Reports_AssetCheckId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReject",
                table: "Reports",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reports",
                table: "Reports",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Reports_ReportId",
                table: "MediaFiles",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AssetChecks_AssetCheckId",
                table: "Reports",
                column: "AssetCheckId",
                principalTable: "AssetChecks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_InventoryChecks_InventoryCheckId",
                table: "Reports",
                column: "InventoryCheckId",
                principalTable: "InventoryChecks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Maintenances_MaintenanceId",
                table: "Reports",
                column: "MaintenanceId",
                principalTable: "Maintenances",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Repairs_RepairId",
                table: "Reports",
                column: "RepairId",
                principalTable: "Repairs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Replacements_ReplacementId",
                table: "Reports",
                column: "ReplacementId",
                principalTable: "Replacements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Transportations_TransportationId",
                table: "Reports",
                column: "TransportationId",
                principalTable: "Transportations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Reports_ReportId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AssetChecks_AssetCheckId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_InventoryChecks_InventoryCheckId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Maintenances_MaintenanceId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Repairs_RepairId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Replacements_ReplacementId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Transportations_TransportationId",
                table: "Reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reports",
                table: "Reports");

            migrationBuilder.RenameTable(
                name: "Reports",
                newName: "Report");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_TransportationId",
                table: "Report",
                newName: "IX_Report_TransportationId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_ReplacementId",
                table: "Report",
                newName: "IX_Report_ReplacementId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_RepairId",
                table: "Report",
                newName: "IX_Report_RepairId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_MaintenanceId",
                table: "Report",
                newName: "IX_Report_MaintenanceId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_InventoryCheckId",
                table: "Report",
                newName: "IX_Report_InventoryCheckId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_AssetCheckId",
                table: "Report",
                newName: "IX_Report_AssetCheckId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReject",
                table: "Report",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Report",
                table: "Report",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Report_ReportId",
                table: "MediaFiles",
                column: "ReportId",
                principalTable: "Report",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_AssetChecks_AssetCheckId",
                table: "Report",
                column: "AssetCheckId",
                principalTable: "AssetChecks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_InventoryChecks_InventoryCheckId",
                table: "Report",
                column: "InventoryCheckId",
                principalTable: "InventoryChecks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Maintenances_MaintenanceId",
                table: "Report",
                column: "MaintenanceId",
                principalTable: "Maintenances",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Repairs_RepairId",
                table: "Report",
                column: "RepairId",
                principalTable: "Repairs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Replacements_ReplacementId",
                table: "Report",
                column: "ReplacementId",
                principalTable: "Replacements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Report_Transportations_TransportationId",
                table: "Report",
                column: "TransportationId",
                principalTable: "Transportations",
                principalColumn: "Id");
        }
    }
}
