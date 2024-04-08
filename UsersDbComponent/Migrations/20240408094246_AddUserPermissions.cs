using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthFrontend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthGroups",
                columns: table => new
                {
                    AuthGroupName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthGroups", x => x.AuthGroupName);
                });

            migrationBuilder.CreateTable(
                name: "AuthPermissions",
                columns: table => new
                {
                    AuthPermissionName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthPermissions", x => x.AuthPermissionName);
                });

            migrationBuilder.CreateTable(
                name: "AuthUserGroups",
                columns: table => new
                {
                    AuthUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthGroupName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUserGroups", x => new { x.AuthUserId, x.AuthGroupName });
                    table.ForeignKey(
                        name: "FK_AuthUserGroups_AuthGroups_AuthGroupName",
                        column: x => x.AuthGroupName,
                        principalTable: "AuthGroups",
                        principalColumn: "AuthGroupName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthUserGroups_AuthUsers_AuthUserId",
                        column: x => x.AuthUserId,
                        principalTable: "AuthUsers",
                        principalColumn: "AuthUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthGroupPermissions",
                columns: table => new
                {
                    AuthGroupName = table.Column<string>(type: "text", nullable: false),
                    AuthPermissionName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthGroupPermissions", x => new { x.AuthGroupName, x.AuthPermissionName });
                    table.ForeignKey(
                        name: "FK_AuthGroupPermissions_AuthGroups_AuthGroupName",
                        column: x => x.AuthGroupName,
                        principalTable: "AuthGroups",
                        principalColumn: "AuthGroupName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthGroupPermissions_AuthPermissions_AuthPermissionName",
                        column: x => x.AuthPermissionName,
                        principalTable: "AuthPermissions",
                        principalColumn: "AuthPermissionName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthGroupPermissions_AuthPermissionName",
                table: "AuthGroupPermissions",
                column: "AuthPermissionName");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUserGroups_AuthGroupName",
                table: "AuthUserGroups",
                column: "AuthGroupName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthGroupPermissions");

            migrationBuilder.DropTable(
                name: "AuthUserGroups");

            migrationBuilder.DropTable(
                name: "AuthPermissions");

            migrationBuilder.DropTable(
                name: "AuthGroups");
        }
    }
}
