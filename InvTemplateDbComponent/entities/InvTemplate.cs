using Microsoft.EntityFrameworkCore;

namespace InvTemplateDbComponent.entities
{
    [PrimaryKey(nameof(InvTemplateName), nameof(InvTemplateVersion))]
    public class InvTemplate
    {
        public required string InvTemplateName { get; set; }
        public uint InvTemplateVersion { get; set; }
        public bool Released { get; set; }

        public IList<InvTemplateAttr> InvTemplateAttr { get; set; } = [];
        public IList<InvTemplateEntAttr> InvTemplateEntAttr { get; set; } = [];
    }
}
