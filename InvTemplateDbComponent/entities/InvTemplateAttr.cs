using Microsoft.EntityFrameworkCore;

namespace InvTemplateDbComponent.entities
{
    [PrimaryKey(nameof(InvTemplateName), nameof(InvTemplateVersion), nameof(InvTemplateAttrName))]
    public class InvTemplateAttr
    {
        public required string InvTemplateName { get; set; }
        public uint InvTemplateVersion { get; set; }
        public required string InvTemplateAttrName { get; set; }
        public required string InvTemplateAttrValue { get; set; }
        public required string InvTemplateAttrAction { get; set; }

        public required InvTemplate InvTemplate { get; set; }
        public IList<InvTemplateAttrPermission> InvTemplateAttrPermissions { get; set; } = [];
    }
}
