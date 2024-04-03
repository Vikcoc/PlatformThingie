using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryDbComponent.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryEntities",
                columns: table => new
                {
                    InventoryEntityId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryEntities", x => x.InventoryEntityId);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTemplates",
                columns: table => new
                {
                    InventoryTemplateName = table.Column<string>(type: "text", nullable: false),
                    InventoryTemplateVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTemplates", x => new { x.InventoryTemplateName, x.InventoryTemplateVersion });
                });

            migrationBuilder.CreateTable(
                name: "InventoryEntityHistories",
                columns: table => new
                {
                    InventoryEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false),
                    AuthUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Log = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryEntityHistories", x => new { x.InventoryEntityId, x.Timestamp });
                    table.ForeignKey(
                        name: "FK_InventoryEntityHistories_InventoryEntities_InventoryEntityId",
                        column: x => x.InventoryEntityId,
                        principalTable: "InventoryEntities",
                        principalColumn: "InventoryEntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTemplateAttributes",
                columns: table => new
                {
                    InventoryTemplateName = table.Column<string>(type: "text", nullable: false),
                    InventoryTemplateVersion = table.Column<long>(type: "bigint", nullable: false),
                    InventoryTemplateAttributeName = table.Column<string>(type: "text", nullable: false),
                    InventoryTemplateAttributeValue = table.Column<string>(type: "text", nullable: false),
                    InventoryTemplateAttributeAction = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTemplateAttributes", x => new { x.InventoryTemplateName, x.InventoryTemplateVersion, x.InventoryTemplateAttributeName });
                    table.ForeignKey(
                        name: "FK_InventoryTemplateAttributes_InventoryTemplates_InventoryTem~",
                        columns: x => new { x.InventoryTemplateName, x.InventoryTemplateVersion },
                        principalTable: "InventoryTemplates",
                        principalColumns: new[] { "InventoryTemplateName", "InventoryTemplateVersion" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTemplateEntityAttributes",
                columns: table => new
                {
                    InventoryTemplateName = table.Column<string>(type: "text", nullable: false),
                    InventoryTemplateVersion = table.Column<long>(type: "bigint", nullable: false),
                    InventoryTemplateEntityAttributeName = table.Column<string>(type: "text", nullable: false),
                    InventoryTemplateEntityAttributeAction = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTemplateEntityAttributes", x => new { x.InventoryTemplateName, x.InventoryTemplateVersion, x.InventoryTemplateEntityAttributeName });
                    table.ForeignKey(
                        name: "FK_InventoryTemplateEntityAttributes_InventoryTemplates_Invent~",
                        columns: x => new { x.InventoryTemplateName, x.InventoryTemplateVersion },
                        principalTable: "InventoryTemplates",
                        principalColumns: new[] { "InventoryTemplateName", "InventoryTemplateVersion" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTemplateAttributeReads",
                columns: table => new
                {
                    InventoryTemplateName = table.Column<string>(type: "text", nullable: false),
                    InventoryTemplateVersion = table.Column<long>(type: "bigint", nullable: false),
                    InventoryTemplateAttributeName = table.Column<string>(type: "text", nullable: false),
                    Permission = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTemplateAttributeReads", x => new { x.InventoryTemplateName, x.InventoryTemplateVersion, x.InventoryTemplateAttributeName, x.Permission });
                    table.ForeignKey(
                        name: "FK_InventoryTemplateAttributeReads_InventoryTemplateAttributes~",
                        columns: x => new { x.InventoryTemplateName, x.InventoryTemplateVersion, x.InventoryTemplateAttributeName },
                        principalTable: "InventoryTemplateAttributes",
                        principalColumns: new[] { "InventoryTemplateName", "InventoryTemplateVersion", "InventoryTemplateAttributeName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryEntitiesAttributeValues",
                columns: table => new
                {
                    InventoryEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryTemplateName = table.Column<string>(type: "text", nullable: false),
                    InventoryTemplateVersion = table.Column<long>(type: "bigint", nullable: false),
                    InventoryTemplateEntityAttributeName = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryEntitiesAttributeValues", x => new { x.InventoryEntityId, x.InventoryTemplateName, x.InventoryTemplateVersion, x.InventoryTemplateEntityAttributeName });
                    table.ForeignKey(
                        name: "FK_InventoryEntitiesAttributeValues_InventoryEntities_Inventor~",
                        column: x => x.InventoryEntityId,
                        principalTable: "InventoryEntities",
                        principalColumn: "InventoryEntityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryEntitiesAttributeValues_InventoryTemplateEntityAtt~",
                        columns: x => new { x.InventoryTemplateName, x.InventoryTemplateVersion, x.InventoryTemplateEntityAttributeName },
                        principalTable: "InventoryTemplateEntityAttributes",
                        principalColumns: new[] { "InventoryTemplateName", "InventoryTemplateVersion", "InventoryTemplateEntityAttributeName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTemplateEntityAttributesPermissions",
                columns: table => new
                {
                    InventoryTemplateName = table.Column<string>(type: "text", nullable: false),
                    InventoryTemplateVersion = table.Column<long>(type: "bigint", nullable: false),
                    InventoryTemplateEntityAttributeName = table.Column<string>(type: "text", nullable: false),
                    Permission = table.Column<string>(type: "text", nullable: false),
                    Writeable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTemplateEntityAttributesPermissions", x => new { x.InventoryTemplateName, x.InventoryTemplateVersion, x.InventoryTemplateEntityAttributeName, x.Permission });
                    table.ForeignKey(
                        name: "FK_InventoryTemplateEntityAttributesPermissions_InventoryTempl~",
                        columns: x => new { x.InventoryTemplateName, x.InventoryTemplateVersion, x.InventoryTemplateEntityAttributeName },
                        principalTable: "InventoryTemplateEntityAttributes",
                        principalColumns: new[] { "InventoryTemplateName", "InventoryTemplateVersion", "InventoryTemplateEntityAttributeName" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryEntitiesAttributeValues_InventoryTemplateName_Inve~",
                table: "InventoryEntitiesAttributeValues",
                columns: new[] { "InventoryTemplateName", "InventoryTemplateVersion", "InventoryTemplateEntityAttributeName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryEntitiesAttributeValues");

            migrationBuilder.DropTable(
                name: "InventoryEntityHistories");

            migrationBuilder.DropTable(
                name: "InventoryTemplateAttributeReads");

            migrationBuilder.DropTable(
                name: "InventoryTemplateEntityAttributesPermissions");

            migrationBuilder.DropTable(
                name: "InventoryEntities");

            migrationBuilder.DropTable(
                name: "InventoryTemplateAttributes");

            migrationBuilder.DropTable(
                name: "InventoryTemplateEntityAttributes");

            migrationBuilder.DropTable(
                name: "InventoryTemplates");
        }
    }
}
