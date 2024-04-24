using System.ComponentModel.DataAnnotations;

namespace InvTemplateDbComponent.entities
{
    public class InvTemplatePermission
    {
        [Key]
        public required string InvTemplatePermissionName { get; set; }

        public IList<InvTemplateAttrPermission> InvTemplateAttrPermissions { get; set; } = [];
        public IList<InvTemplateEntAttrPermission> InvTemplateEntAttrPermissions { get; set; } = [];
    }
}
