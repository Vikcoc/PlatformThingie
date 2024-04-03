using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryDbComponent.entities
{
    [PrimaryKey(nameof(InventoryEntityId), nameof(Timestamp))]
    public class InventoryEntityHistory
    {
        [ForeignKey(nameof(InventoryEntity))]
        public Guid InventoryEntityId { get; set; }
        public long Timestamp {  get; set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        public required Guid AuthUserId { get; set; }
        public required string Log {  get; set; }

        public required InventoryEntity InventoryEntity { get; set; }
    }
}
