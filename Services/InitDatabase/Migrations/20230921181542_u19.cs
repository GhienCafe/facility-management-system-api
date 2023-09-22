using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Users_PersonInChargeId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Repairation_Users_CreatorId",
                table: "Repairation");

            migrationBuilder.DropForeignKey(
                name: "FK_Replacements_Users_CreatorId",
                table: "Replacements");

            migrationBuilder.DropIndex(
                name: "IX_Replacements_CreatorId",
                table: "Replacements");

            migrationBuilder.DropIndex(
                name: "IX_Repairation_CreatorId",
                table: "Repairation");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_PersonInChargeId",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "PersonInChargeId",
                table: "Maintenances");

            migrationBuilder.AddColumn<Guid>(
                name: "PersonInChargeId",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transportations_PersonInChargeId",
                table: "Transportations",
                column: "PersonInChargeId");

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
                name: "FK_Transportations_Users_PersonInChargeId",
                table: "Transportations",
                column: "PersonInChargeId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Users_AssignedTo",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Transportations_Users_PersonInChargeId",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Transportations_PersonInChargeId",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_AssignedTo",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "PersonInChargeId",
                table: "Transportations");

            migrationBuilder.AddColumn<Guid>(
                name: "PersonInChargeId",
                table: "Maintenances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Replacements_CreatorId",
                table: "Replacements",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Repairation_CreatorId",
                table: "Repairation",
                column: "CreatorId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Repairation_Users_CreatorId",
                table: "Repairation",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Replacements_Users_CreatorId",
                table: "Replacements",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
