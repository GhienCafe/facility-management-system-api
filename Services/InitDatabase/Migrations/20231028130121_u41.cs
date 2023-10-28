using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u41 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Piority",
                table: "Transportations",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "Piority",
                table: "Replacements",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "Piority",
                table: "Repairations",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "Piority",
                table: "Maintenances",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "Piority",
                table: "AssetChecks",
                newName: "Priority");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "Transportations",
                newName: "Piority");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "Replacements",
                newName: "Piority");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "Repairations",
                newName: "Piority");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "Maintenances",
                newName: "Piority");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "AssetChecks",
                newName: "Piority");
        }
    }
}
