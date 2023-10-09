using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u32 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssetType",
                table: "Transportations",
                newName: "AssetTypeId");

            migrationBuilder.RenameColumn(
                name: "AssetType",
                table: "Replacements",
                newName: "AssetTypeId");

            migrationBuilder.RenameColumn(
                name: "AssetType",
                table: "Repairations",
                newName: "AssetTypeId");

            migrationBuilder.RenameColumn(
                name: "AssetType",
                table: "Maintenances",
                newName: "AssetTypeId");

            migrationBuilder.RenameColumn(
                name: "AssetType",
                table: "AssetChecks",
                newName: "AssetTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AssetTypeId",
                table: "Transportations",
                newName: "AssetType");

            migrationBuilder.RenameColumn(
                name: "AssetTypeId",
                table: "Replacements",
                newName: "AssetType");

            migrationBuilder.RenameColumn(
                name: "AssetTypeId",
                table: "Repairations",
                newName: "AssetType");

            migrationBuilder.RenameColumn(
                name: "AssetTypeId",
                table: "Maintenances",
                newName: "AssetType");

            migrationBuilder.RenameColumn(
                name: "AssetTypeId",
                table: "AssetChecks",
                newName: "AssetType");
        }
    }
}
