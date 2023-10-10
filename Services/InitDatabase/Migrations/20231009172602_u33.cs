using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u33 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "RoomStatus");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Rooms",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Rooms");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "RoomStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
