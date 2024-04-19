using Microsoft.EntityFrameworkCore;

namespace InvTemplateDbComponent.entities
{
    [PrimaryKey(nameof(InvTemplateName), nameof(InvTemplateVersion), nameof(InvTemplateEntAttrName), nameof(InvTemplatePermissionName))]
    public class InvTemplateEntAttrPermission
    {
        public required string InvTemplateName { get; set; }
        public uint InvTemplateVersion { get; set; }
        public required string InvTemplateEntAttrName { get; set; }
        public required string InvTemplatePermissionName { get; set; }
        public bool Writeable { get; set; }

        public required InvTemplatePermission InvTemplatePermission { get; set; }
        public required InvTemplateEntAttr InvTemplateEntAttr { get; set; }
    }
}
