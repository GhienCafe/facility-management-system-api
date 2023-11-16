using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u61 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "InventoryCheckDetails",
                newName: "StatusReported");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "InventoryCheckDetails",
                newName: "QuantityReported");

            migrationBuilder.AddColumn<double>(
                name: "QuantityBefore",
                table: "InventoryCheckDetails",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusBefore",
                table: "InventoryCheckDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantityBefore",
                table: "InventoryCheckDetails");

            migrationBuilder.DropColumn(
                name: "StatusBefore",
                table: "InventoryCheckDetails");

            migrationBuilder.RenameColumn(
                name: "StatusReported",
                table: "InventoryCheckDetails",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "QuantityReported",
                table: "InventoryCheckDetails",
                newName: "Quantity");
        }
    }
}
