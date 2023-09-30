using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestCode",
                table: "Transportations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestCode",
                table: "Replacements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestCode",
                table: "Repairation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestCode",
                table: "Maintenances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FloorName",
                table: "Floors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestCode",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "RequestCode",
                table: "Replacements");

            migrationBuilder.DropColumn(
                name: "RequestCode",
                table: "Repairation");

            migrationBuilder.DropColumn(
                name: "RequestCode",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "FloorName",
                table: "Floors");
        }
    }
}
