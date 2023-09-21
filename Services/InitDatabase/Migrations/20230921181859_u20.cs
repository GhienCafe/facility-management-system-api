using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transportations_Users_PersonInChargeId",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Transportations_PersonInChargeId",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "PersonInChargeId",
                table: "Transportations");

            migrationBuilder.CreateIndex(
                name: "IX_Transportations_AssignedTo",
                table: "Transportations",
                column: "AssignedTo");

            migrationBuilder.AddForeignKey(
                name: "FK_Transportations_Users_AssignedTo",
                table: "Transportations",
                column: "AssignedTo",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transportations_Users_AssignedTo",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Transportations_AssignedTo",
                table: "Transportations");

            migrationBuilder.AddColumn<Guid>(
                name: "PersonInChargeId",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transportations_PersonInChargeId",
                table: "Transportations",
                column: "PersonInChargeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transportations_Users_PersonInChargeId",
                table: "Transportations",
                column: "PersonInChargeId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
