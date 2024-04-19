using Microsoft.EntityFrameworkCore;

namespace InvTemplateDbComponent.entities
{
    public class InvTemplateContext : DbContext
    {
        public InvTemplateContext() : base(new DbContextOptionsBuilder().UseNpgsql().Options) { }

        public InvTemplateContext(DbContextOptions options) : base(options) { }

        public DbSet<InvTemplatePermission> InvTemplatePermissions { get; set; }
        public DbSet<InvTemplateEntAttrPermission> InvTemplateEntAttrPermissions { get; set; }
        public DbSet<InvTemplateEntAttr> InvTemplateEntAttrs { get; set; }
        public DbSet<InvTemplate> InvTemplates { get; set; }
        public DbSet<InvTemplateAttr> InvTemplatesAttrs { get; set; }
        public DbSet<InvTemplateAttrPermission> InvTemplatesAttrPermissions { get; set; }
    }
}
