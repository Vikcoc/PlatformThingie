using System.ComponentModel.DataAnnotations;

namespace InventoryDbComponent.entities
{
    public class InventoryEntity
    {
        [Key]
        public Guid InventoryEntityId { get; set; } = Guid.NewGuid();

        public IList<InventoryEntityAttributeValue> InventoryEntityAttributeValues { get; set; } = [];
        public IList<InventoryEntityHistory> InventoryEntityHistories { get; set; } = [];
    }
}
