using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthFrontend.Migrations
{
    /// <inheritdoc />
    public partial class AuthClameUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClaimValue",
                table: "AuthUserClaims",
                newName: "AuthClaimValue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AuthClaimValue",
                table: "AuthUserClaims",
                newName: "ClaimValue");
        }
    }
}
