using System;
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
            migrationBuilder.DropForeignKey(
                name: "FK_Transportations_Assets_AssetId",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Transportations_AssetId",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "AssetTypeId",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Transportations");

            migrationBuilder.AlterColumn<string>(
                name: "RequestCode",
                table: "Transportations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Transportations_RequestCode",
                table: "Transportations",
                column: "RequestCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transportations_RequestCode",
                table: "Transportations");

            migrationBuilder.AlterColumn<string>(
                name: "RequestCode",
                table: "Transportations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "AssetId",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetTypeId",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transportations_AssetId",
                table: "Transportations",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transportations_Assets_AssetId",
                table: "Transportations",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");
        }
    }
}
