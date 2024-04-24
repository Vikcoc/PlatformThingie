using Microsoft.EntityFrameworkCore;

namespace InvTemplateDbComponent.entities
{
    [PrimaryKey(nameof(InvTemplateName), nameof(InvTemplateVersion), nameof(InvTemplateAttrName), nameof(InvTemplatePermissionName))]
    public class InvTemplateAttrPermission
    {
        public required string InvTemplateName { get; set; }
        public uint InvTemplateVersion { get; set; }
        public required string InvTemplateAttrName { get; set; }
        public required string InvTemplatePermissionName { get; set; }

        public required InvTemplatePermission InvTemplatePermission { get; set; }
        public required InvTemplateAttr InvTemplateAttr { get; set; }
    }
}
