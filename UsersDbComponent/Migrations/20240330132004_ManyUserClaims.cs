using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthFrontend.Migrations
{
    /// <inheritdoc />
    public partial class ManyUserClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthUsers_AuthUsers_NewAuthUserId",
                table: "AuthUsers");

            migrationBuilder.DropIndex(
                name: "IX_AuthUsers_NewAuthUserId",
                table: "AuthUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthUserClaims",
                table: "AuthUserClaims");

            migrationBuilder.DropColumn(
                name: "NewAuthUserId",
                table: "AuthUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthUserClaimId",
                table: "AuthUserClaims",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthUserClaims",
                table: "AuthUserClaims",
                column: "AuthUserClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUserClaims_AuthUserId",
                table: "AuthUserClaims",
                column: "AuthUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthUserClaims",
                table: "AuthUserClaims");

            migrationBuilder.DropIndex(
                name: "IX_AuthUserClaims_AuthUserId",
                table: "AuthUserClaims");

            migrationBuilder.DropColumn(
                name: "AuthUserClaimId",
                table: "AuthUserClaims");

            migrationBuilder.AddColumn<Guid>(
                name: "NewAuthUserId",
                table: "AuthUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthUserClaims",
                table: "AuthUserClaims",
                columns: new[] { "AuthUserId", "AuthClaimName" });

            migrationBuilder.CreateIndex(
                name: "IX_AuthUsers_NewAuthUserId",
                table: "AuthUsers",
                column: "NewAuthUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthUsers_AuthUsers_NewAuthUserId",
                table: "AuthUsers",
                column: "NewAuthUserId",
                principalTable: "AuthUsers",
                principalColumn: "AuthUserId");
        }
    }
}
