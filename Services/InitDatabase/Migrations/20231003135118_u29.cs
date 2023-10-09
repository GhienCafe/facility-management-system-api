using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InitDatabase.Migrations
{
    /// <inheritdoc />
    public partial class u29 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetCheck_Assets_AssetId",
                table: "AssetCheck");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetCheck_Requests_RequestId",
                table: "AssetCheck");

            migrationBuilder.DropForeignKey(
                name: "FK_Repairation_Assets_AssetId",
                table: "Repairation");

            migrationBuilder.DropForeignKey(
                name: "FK_Repairation_Requests_RequestId",
                table: "Repairation");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMember_Teams_TeamId",
                table: "TeamMember");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMember_Users_MemberId",
                table: "TeamMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMember",
                table: "TeamMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Repairation",
                table: "Repairation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetCheck",
                table: "AssetCheck");

            migrationBuilder.RenameTable(
                name: "TeamMember",
                newName: "TeamMembers");

            migrationBuilder.RenameTable(
                name: "Repairation",
                newName: "Repairations");

            migrationBuilder.RenameTable(
                name: "AssetCheck",
                newName: "AssetChecks");

            migrationBuilder.RenameIndex(
                name: "IX_TeamMember_TeamId",
                table: "TeamMembers",
                newName: "IX_TeamMembers_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_TeamMember_MemberId",
                table: "TeamMembers",
                newName: "IX_TeamMembers_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Repairation_RequestId",
                table: "Repairations",
                newName: "IX_Repairations_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Repairation_AssetId",
                table: "Repairations",
                newName: "IX_Repairations_AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetCheck_RequestId",
                table: "AssetChecks",
                newName: "IX_AssetChecks_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetCheck_AssetId",
                table: "AssetChecks",
                newName: "IX_AssetChecks_AssetId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLead",
                table: "TeamMembers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "Repairations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AssetId",
                table: "Repairations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Repairations",
                table: "Repairations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetChecks",
                table: "AssetChecks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetChecks_Assets_AssetId",
                table: "AssetChecks",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetChecks_Requests_RequestId",
                table: "AssetChecks",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Repairations_Assets_AssetId",
                table: "Repairations",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Repairations_Requests_RequestId",
                table: "Repairations",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Teams_TeamId",
                table: "TeamMembers",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMembers_Users_MemberId",
                table: "TeamMembers",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetChecks_Assets_AssetId",
                table: "AssetChecks");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetChecks_Requests_RequestId",
                table: "AssetChecks");

            migrationBuilder.DropForeignKey(
                name: "FK_Repairations_Assets_AssetId",
                table: "Repairations");

            migrationBuilder.DropForeignKey(
                name: "FK_Repairations_Requests_RequestId",
                table: "Repairations");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Teams_TeamId",
                table: "TeamMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMembers_Users_MemberId",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeamMembers",
                table: "TeamMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Repairations",
                table: "Repairations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetChecks",
                table: "AssetChecks");

            migrationBuilder.RenameTable(
                name: "TeamMembers",
                newName: "TeamMember");

            migrationBuilder.RenameTable(
                name: "Repairations",
                newName: "Repairation");

            migrationBuilder.RenameTable(
                name: "AssetChecks",
                newName: "AssetCheck");

            migrationBuilder.RenameIndex(
                name: "IX_TeamMembers_TeamId",
                table: "TeamMember",
                newName: "IX_TeamMember_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_TeamMembers_MemberId",
                table: "TeamMember",
                newName: "IX_TeamMember_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Repairations_RequestId",
                table: "Repairation",
                newName: "IX_Repairation_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_Repairations_AssetId",
                table: "Repairation",
                newName: "IX_Repairation_AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetChecks_RequestId",
                table: "AssetCheck",
                newName: "IX_AssetCheck_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetChecks_AssetId",
                table: "AssetCheck",
                newName: "IX_AssetCheck_AssetId");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLead",
                table: "TeamMember",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "Repairation",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssetId",
                table: "Repairation",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeamMember",
                table: "TeamMember",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Repairation",
                table: "Repairation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetCheck",
                table: "AssetCheck",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCheck_Assets_AssetId",
                table: "AssetCheck",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetCheck_Requests_RequestId",
                table: "AssetCheck",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Repairation_Assets_AssetId",
                table: "Repairation",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Repairation_Requests_RequestId",
                table: "Repairation",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMember_Teams_TeamId",
                table: "TeamMember",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMember_Users_MemberId",
                table: "TeamMember",
                column: "MemberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
