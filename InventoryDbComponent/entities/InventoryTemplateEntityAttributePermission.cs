using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryDbComponent.entities
{
    [PrimaryKey(nameof(InventoryTemplateName), nameof(InventoryTemplateVersion), nameof(InventoryTemplateEntityAttributeName), nameof(Permission))]
    public class InventoryTemplateEntityAttributePermission
    {
        //[ForeignKey(nameof(InventoryTemplateEntityAttribute))]
        public required string InventoryTemplateName { get; set; }
        //[ForeignKey(nameof(InventoryTemplateEntityAttribute))]
        public uint InventoryTemplateVersion { get; set; }
        //[ForeignKey(nameof(InventoryTemplateEntityAttribute))]
        public required string InventoryTemplateEntityAttributeName { get; set; }
        public required string Permission { get; set; }
        public bool Writeable { get; set; }

        public required InventoryTemplateEntityAttribute InventoryTemplateEntityAttribute { get; set; }
    }
}
