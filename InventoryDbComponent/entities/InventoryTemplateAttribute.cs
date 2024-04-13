using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryDbComponent.entities
{
    [PrimaryKey(nameof(InventoryTemplateName), nameof(InventoryTemplateVersion), nameof(InventoryTemplateAttributeName))]
    public class InventoryTemplateAttribute
    {
        //[ForeignKey(nameof(InventoryTemplate))]
        public required string InventoryTemplateName { get; set; }
        //[ForeignKey(nameof(InventoryTemplate))]
        public uint InventoryTemplateVersion { get; set; }
        public required string InventoryTemplateAttributeName { get; set; }
        public required string InventoryTemplateAttributeValue { get; set; }
        public string? InventoryTemplateAttributeAction { get; set; }

        public required InventoryTemplate InventoryTemplate {  get; set; }
        public IList<InventoryTemplateAttributeRead> InventoryTemplateAttributeReads { get; set; } = [];
    }
}
