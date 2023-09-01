using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ColorStatusId",
                table: "Rooms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ColorStatus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_ColorStatusId",
                table: "Rooms",
                column: "ColorStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_ColorStatus_ColorStatusId",
                table: "Rooms",
                column: "ColorStatusId",
                principalTable: "ColorStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_ColorStatus_ColorStatusId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "ColorStatus");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_ColorStatusId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ColorStatusId",
                table: "Rooms");
        }
    }
}
