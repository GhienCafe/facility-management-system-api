using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u44 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_MaintenanceScheduleConfigs_MaintenanceConfigId",
                table: "Assets");

            migrationBuilder.DropTable(
                name: "MaintenanceScheduleConfigs");

            migrationBuilder.DropIndex(
                name: "IX_Assets_MaintenanceConfigId",
                table: "Assets");

            migrationBuilder.AddColumn<int>(
                name: "MaintenancePeriodTime",
                table: "Models",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaintenancePeriodTime",
                table: "Models");

            migrationBuilder.CreateTable(
                name: "MaintenanceScheduleConfigs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RepeatIntervalInMonths = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceScheduleConfigs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_MaintenanceConfigId",
                table: "Assets",
                column: "MaintenanceConfigId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_MaintenanceScheduleConfigs_MaintenanceConfigId",
                table: "Assets",
                column: "MaintenanceConfigId",
                principalTable: "MaintenanceScheduleConfigs",
                principalColumn: "Id");
        }
    }
}
