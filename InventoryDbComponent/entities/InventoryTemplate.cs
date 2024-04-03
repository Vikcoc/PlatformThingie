using Microsoft.EntityFrameworkCore;

namespace InventoryDbComponent.entities
{
    [PrimaryKey(nameof(InventoryTemplateName), nameof(InventoryTemplateVersion))]
    public class InventoryTemplate
    {
        public required string InventoryTemplateName { get; set; }
        public uint InventoryTemplateVersion { get; set; }

        public IList<InventoryTemplateEntityAttribute> InventoryTemplateEntityAttributes { get; set; } = [];
        public IList<InventoryTemplateAttribute> InventoryTemplateAttributes { get; set; } = [];
    }
}
