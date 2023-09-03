using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryTeamMember",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventoryTeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EditorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTeamMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryTeamMember_InventoryTeam_InventoryTeamId",
                        column: x => x.InventoryTeamId,
                        principalTable: "InventoryTeam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryTeamMember_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTeamMember_InventoryTeamId",
                table: "InventoryTeamMember",
                column: "InventoryTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTeamMember_UserId",
                table: "InventoryTeamMember",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryTeamMember");
        }
    }
}
