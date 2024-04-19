using Microsoft.EntityFrameworkCore;

namespace InvTemplateDbComponent.entities
{
    [PrimaryKey(nameof(InvTemplateName), nameof(InvTemplateVersion), nameof(InvTemplateEntAttrName))]
    public class InvTemplateEntAttr
    {
        public required string InvTemplateName { get; set; }
        public uint InvTemplateVersion { get; set; }
        public required string InvTemplateEntAttrName { get; set; }
        public required string InvTemplateEntAttrAction { get; set; }

        public required InvTemplate InvTemplate { get; set; }
        public IList<InvTemplateEntAttrPermission> InvTemplateEntAttrPermissions { get; set; } = [];
    }
}
