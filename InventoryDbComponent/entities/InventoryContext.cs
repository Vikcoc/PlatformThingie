using Microsoft.EntityFrameworkCore;

namespace InventoryDbComponent.entities
{
    public class InventoryContext : DbContext
    {
        public InventoryContext() : base(new DbContextOptionsBuilder().UseNpgsql().Options) { }

        public InventoryContext(DbContextOptions options) : base(options) { }

        public DbSet<InventoryEntityHistory> InventoryEntityHistories { get; set; }
        public DbSet<InventoryEntity> InventoryEntities { get; set; }
        public DbSet<InventoryEntityAttributeValue> InventoryEntitiesAttributeValues { get; set; }
        public DbSet<InventoryTemplateEntityAttribute> InventoryTemplateEntityAttributes { get; set; }
        public DbSet<InventoryTemplateEntityAttributePermission> InventoryTemplateEntityAttributesPermissions { get; set; }
        public DbSet<InventoryTemplate> InventoryTemplates { get; set; }
        public DbSet<InventoryTemplateAttribute> InventoryTemplateAttributes { get; set; }
        public DbSet<InventoryTemplateAttributeRead> InventoryTemplateAttributeReads { get; set; }
    }
}
