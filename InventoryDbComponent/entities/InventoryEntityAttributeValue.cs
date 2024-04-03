using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryDbComponent.entities
{
    [PrimaryKey(nameof(InventoryEntityId), nameof(InventoryTemplateName), nameof(InventoryTemplateVersion), nameof(InventoryTemplateEntityAttributeName))]
    public class InventoryEntityAttributeValue
    {
        // todo: Devise a way to store scripts for the action probably js scripts for read and c# methods/objects for write?, that look for roles
        [ForeignKey(nameof(InventoryEntity))]
        public Guid InventoryEntityId { get; set; }
        //[ForeignKey(nameof(InventoryTemplateEntityAttribute))]
        public required string InventoryTemplateName { get; set; }
        //[ForeignKey(nameof(InventoryTemplateEntityAttribute))]
        public uint InventoryTemplateVersion { get; set; }
        //[ForeignKey(nameof(InventoryTemplateEntityAttribute))]
        public required string InventoryTemplateEntityAttributeName { get; set; }
        public required string Value {  get; set; }

        public required InventoryEntity InventoryEntity { get; set; }
        public required InventoryTemplateEntityAttribute InventoryTemplateEntityAttribute { get; set; }
    }
}
