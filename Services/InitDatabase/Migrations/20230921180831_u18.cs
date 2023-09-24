using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Users_AssignedTo",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Transportations_Users_CreatorId",
                table: "Transportations");

            migrationBuilder.DropForeignKey(
                name: "FK_Transportations_Users_CreatorId1",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Transportations_CreatorId",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Transportations_CreatorId1",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_AssignedTo",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "CreatorId1",
                table: "Transportations");

            migrationBuilder.AddColumn<Guid>(
                name: "PersonInChargeId",
                table: "Maintenances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_PersonInChargeId",
                table: "Maintenances",
                column: "PersonInChargeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Users_PersonInChargeId",
                table: "Maintenances",
                column: "PersonInChargeId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Users_PersonInChargeId",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_PersonInChargeId",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "PersonInChargeId",
                table: "Maintenances");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId1",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transportations_CreatorId",
                table: "Transportations",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Transportations_CreatorId1",
                table: "Transportations",
                column: "CreatorId1");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_AssignedTo",
                table: "Maintenances",
                column: "AssignedTo");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Users_AssignedTo",
                table: "Maintenances",
                column: "AssignedTo",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transportations_Users_CreatorId",
                table: "Transportations",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transportations_Users_CreatorId1",
                table: "Transportations",
                column: "CreatorId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
