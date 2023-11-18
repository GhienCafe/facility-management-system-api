using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u63 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryChecks_Assets_AssetId",
                table: "InventoryChecks");

            migrationBuilder.DropForeignKey(
                name: "FK_Replacements_Assets_NewAssetId",
                table: "Replacements");

            migrationBuilder.DropForeignKey(
                name: "FK_Transportations_Assets_AssetId",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Transportations_AssetId",
                table: "Transportations");

            migrationBuilder.DropIndex(
                name: "IX_Replacements_NewAssetId",
                table: "Replacements");

            migrationBuilder.DropIndex(
                name: "IX_InventoryChecks_AssetId",
                table: "InventoryChecks");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "Transportations");

            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "InventoryChecks");

            migrationBuilder.CreateIndex(
                name: "IX_Replacements_AssetId",
                table: "Replacements",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Replacements_Assets_AssetId",
                table: "Replacements",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Replacements_Assets_AssetId",
                table: "Replacements");

            migrationBuilder.DropIndex(
                name: "IX_Replacements_AssetId",
                table: "Replacements");

            migrationBuilder.AddColumn<Guid>(
                name: "AssetId",
                table: "Transportations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssetId",
                table: "InventoryChecks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transportations_AssetId",
                table: "Transportations",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Replacements_NewAssetId",
                table: "Replacements",
                column: "NewAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryChecks_AssetId",
                table: "InventoryChecks",
                column: "AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryChecks_Assets_AssetId",
                table: "InventoryChecks",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Replacements_Assets_NewAssetId",
                table: "Replacements",
                column: "NewAssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transportations_Assets_AssetId",
                table: "Transportations",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");
        }
    }
}
