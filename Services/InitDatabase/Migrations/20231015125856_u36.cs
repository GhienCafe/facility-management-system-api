using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u36 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Period",
                table: "MaintenanceScheduleConfigs",
                newName: "RepeatIntervalInMonths");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MaintenanceScheduleConfigs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "MaintenanceScheduleConfigs");

            migrationBuilder.RenameColumn(
                name: "RepeatIntervalInMonths",
                table: "MaintenanceScheduleConfigs",
                newName: "Period");
        }
    }
}
