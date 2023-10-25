using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u40 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Piority",
                table: "Transportations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Piority",
                table: "Replacements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Piority",
                table: "Repairations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Piority",
                table: "Maintenances",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Piority",
                table: "AssetChecks",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Piority",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "Piority",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "Piority",
                table: "Repairations");

            migrationBuilder.DropColumn(
                name: "Piority",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Piority",
                table: "AssetChecks");
        }
    }
}
