using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryDbComponent.entities
{
    [PrimaryKey(nameof(InventoryTemplateName), nameof(InventoryTemplateVersion), nameof(InventoryTemplateAttributeName), nameof(Permission))]
    public class InventoryTemplateAttributeRead
    {
        //[ForeignKey(nameof(InventoryTemplateAttribute))]
        public required string InventoryTemplateName { get; set; }
        //[ForeignKey(nameof(InventoryTemplateAttribute))]
        public uint InventoryTemplateVersion { get; set; }
        //[ForeignKey(nameof(InventoryTemplateAttribute))]
        public required string InventoryTemplateAttributeName { get; set; }
        public required string Permission { get; set; }

        public required InventoryTemplateAttribute InventoryTemplateAttribute { get; set; }
    }
}
