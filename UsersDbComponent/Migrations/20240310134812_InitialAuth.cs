using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthFrontend.Migrations
{
    /// <inheritdoc />
    public partial class InitialAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthClaims",
                columns: table => new
                {
                    AuthClaimName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthClaims", x => x.AuthClaimName);
                });

            migrationBuilder.CreateTable(
                name: "AuthUsers",
                columns: table => new
                {
                    AuthUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    NewAuthUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUsers", x => x.AuthUserId);
                    table.ForeignKey(
                        name: "FK_AuthUsers_AuthUsers_NewAuthUserId",
                        column: x => x.NewAuthUserId,
                        principalTable: "AuthUsers",
                        principalColumn: "AuthUserId");
                });

            migrationBuilder.CreateTable(
                name: "AuthUserClaims",
                columns: table => new
                {
                    AuthUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthClaimName = table.Column<string>(type: "text", nullable: false),
                    ClaimValue = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUserClaims", x => new { x.AuthUserId, x.AuthClaimName });
                    table.ForeignKey(
                        name: "FK_AuthUserClaims_AuthClaims_AuthClaimName",
                        column: x => x.AuthClaimName,
                        principalTable: "AuthClaims",
                        principalColumn: "AuthClaimName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthUserClaims_AuthUsers_AuthUserId",
                        column: x => x.AuthUserId,
                        principalTable: "AuthUsers",
                        principalColumn: "AuthUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthUserRefreshTokens",
                columns: table => new
                {
                    JTI = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    HashedToken = table.Column<string>(type: "text", nullable: false),
                    Expire = table.Column<long>(type: "bigint", nullable: false),
                    Salt = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthUserRefreshTokens", x => x.JTI);
                    table.ForeignKey(
                        name: "FK_AuthUserRefreshTokens_AuthUsers_AuthUserId",
                        column: x => x.AuthUserId,
                        principalTable: "AuthUsers",
                        principalColumn: "AuthUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthUserClaims_AuthClaimName",
                table: "AuthUserClaims",
                column: "AuthClaimName");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUserRefreshTokens_AuthUserId",
                table: "AuthUserRefreshTokens",
                column: "AuthUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthUsers_NewAuthUserId",
                table: "AuthUsers",
                column: "NewAuthUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthUserClaims");

            migrationBuilder.DropTable(
                name: "AuthUserRefreshTokens");

            migrationBuilder.DropTable(
                name: "AuthClaims");

            migrationBuilder.DropTable(
                name: "AuthUsers");
        }
    }
}
