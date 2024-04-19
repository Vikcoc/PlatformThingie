using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryDbComponent.entities
{
    [PrimaryKey(nameof(InventoryTemplateName), nameof(InventoryTemplateVersion), nameof(InventoryTemplateEntityAttributeName))]
    public class InventoryTemplateEntityAttribute
    {
        //[ForeignKey(nameof(InventoryTemplate))]
        public required string InventoryTemplateName { get; set; }
        //[ForeignKey(nameof(InventoryTemplate))]
        public uint InventoryTemplateVersion { get; set; }
        public required string InventoryTemplateEntityAttributeName { get; set; }
        public required string InventoryTemplateEntityAttributeAction { get; set; }

        public required InventoryTemplate InventoryTemplate { get; set; }
        public IList<InventoryEntityAttributeValue> InventoryEntityAttributeValues { get; set; } = [];
        public IList<InventoryTemplateEntityAttributePermission> InventoryEntityAttributeValuePermissions { get; set; } = [];
    }
}
