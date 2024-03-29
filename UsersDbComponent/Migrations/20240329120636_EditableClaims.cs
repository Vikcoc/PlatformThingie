using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthFrontend.Migrations
{
    /// <inheritdoc />
    public partial class EditableClaims : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthClaimRight",
                table: "AuthClaims",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthClaimRight",
                table: "AuthClaims");
        }
    }
}
