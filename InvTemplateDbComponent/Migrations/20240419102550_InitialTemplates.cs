using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvTemplateDbComponent.Migrations
{
    /// <inheritdoc />
    public partial class InitialTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvTemplatePermissions",
                columns: table => new
                {
                    InvTemplatePermissionName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvTemplatePermissions", x => x.InvTemplatePermissionName);
                });

            migrationBuilder.CreateTable(
                name: "InvTemplates",
                columns: table => new
                {
                    InvTemplateName = table.Column<string>(type: "text", nullable: false),
                    InvTemplateVersion = table.Column<long>(type: "bigint", nullable: false),
                    Released = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvTemplates", x => new { x.InvTemplateName, x.InvTemplateVersion });
                });

            migrationBuilder.CreateTable(
                name: "InvTemplateEntAttrs",
                columns: table => new
                {
                    InvTemplateName = table.Column<string>(type: "text", nullable: false),
                    InvTemplateVersion = table.Column<long>(type: "bigint", nullable: false),
                    InvTemplateEntAttrName = table.Column<string>(type: "text", nullable: false),
                    InvTemplateEntAttrAction = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvTemplateEntAttrs", x => new { x.InvTemplateName, x.InvTemplateVersion, x.InvTemplateEntAttrName });
                    table.ForeignKey(
                        name: "FK_InvTemplateEntAttrs_InvTemplates_InvTemplateName_InvTemplat~",
                        columns: x => new { x.InvTemplateName, x.InvTemplateVersion },
                        principalTable: "InvTemplates",
                        principalColumns: new[] { "InvTemplateName", "InvTemplateVersion" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvTemplatesAttrs",
                columns: table => new
                {
                    InvTemplateName = table.Column<string>(type: "text", nullable: false),
                    InvTemplateVersion = table.Column<long>(type: "bigint", nullable: false),
                    InvTemplateAttrName = table.Column<string>(type: "text", nullable: false),
                    InvTemplateAttrValue = table.Column<string>(type: "text", nullable: false),
                    InvTemplateAttrAction = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvTemplatesAttrs", x => new { x.InvTemplateName, x.InvTemplateVersion, x.InvTemplateAttrName });
                    table.ForeignKey(
                        name: "FK_InvTemplatesAttrs_InvTemplates_InvTemplateName_InvTemplateV~",
                        columns: x => new { x.InvTemplateName, x.InvTemplateVersion },
                        principalTable: "InvTemplates",
                        principalColumns: new[] { "InvTemplateName", "InvTemplateVersion" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvTemplateEntAttrPermissions",
                columns: table => new
                {
                    InvTemplateName = table.Column<string>(type: "text", nullable: false),
                    InvTemplateVersion = table.Column<long>(type: "bigint", nullable: false),
                    InvTemplateEntAttrName = table.Column<string>(type: "text", nullable: false),
                    InvTemplatePermissionName = table.Column<string>(type: "text", nullable: false),
                    Writeable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvTemplateEntAttrPermissions", x => new { x.InvTemplateName, x.InvTemplateVersion, x.InvTemplateEntAttrName, x.InvTemplatePermissionName });
                    table.ForeignKey(
                        name: "FK_InvTemplateEntAttrPermissions_InvTemplateEntAttrs_InvTempla~",
                        columns: x => new { x.InvTemplateName, x.InvTemplateVersion, x.InvTemplateEntAttrName },
                        principalTable: "InvTemplateEntAttrs",
                        principalColumns: new[] { "InvTemplateName", "InvTemplateVersion", "InvTemplateEntAttrName" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvTemplateEntAttrPermissions_InvTemplatePermissions_InvTem~",
                        column: x => x.InvTemplatePermissionName,
                        principalTable: "InvTemplatePermissions",
                        principalColumn: "InvTemplatePermissionName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvTemplatesAttrPermissions",
                columns: table => new
                {
                    InvTemplateName = table.Column<string>(type: "text", nullable: false),
                    InvTemplateVersion = table.Column<long>(type: "bigint", nullable: false),
                    InvTemplateAttrName = table.Column<string>(type: "text", nullable: false),
                    InvTemplatePermissionName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvTemplatesAttrPermissions", x => new { x.InvTemplateName, x.InvTemplateVersion, x.InvTemplateAttrName, x.InvTemplatePermissionName });
                    table.ForeignKey(
                        name: "FK_InvTemplatesAttrPermissions_InvTemplatePermissions_InvTempl~",
                        column: x => x.InvTemplatePermissionName,
                        principalTable: "InvTemplatePermissions",
                        principalColumn: "InvTemplatePermissionName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvTemplatesAttrPermissions_InvTemplatesAttrs_InvTemplateNa~",
                        columns: x => new { x.InvTemplateName, x.InvTemplateVersion, x.InvTemplateAttrName },
                        principalTable: "InvTemplatesAttrs",
                        principalColumns: new[] { "InvTemplateName", "InvTemplateVersion", "InvTemplateAttrName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvTemplateEntAttrPermissions_InvTemplatePermissionName",
                table: "InvTemplateEntAttrPermissions",
                column: "InvTemplatePermissionName");

            migrationBuilder.CreateIndex(
                name: "IX_InvTemplatesAttrPermissions_InvTemplatePermissionName",
                table: "InvTemplatesAttrPermissions",
                column: "InvTemplatePermissionName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvTemplateEntAttrPermissions");

            migrationBuilder.DropTable(
                name: "InvTemplatesAttrPermissions");

            migrationBuilder.DropTable(
                name: "InvTemplateEntAttrs");

            migrationBuilder.DropTable(
                name: "InvTemplatePermissions");

            migrationBuilder.DropTable(
                name: "InvTemplatesAttrs");

            migrationBuilder.DropTable(
                name: "InvTemplates");
        }
    }
}
