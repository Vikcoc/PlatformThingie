using InventoryDbComponent.entities;
using Microsoft.EntityFrameworkCore;
using PlatformInterfaces;

namespace InventoryDbComponent.seeding
{
    public class Seeder : IMigrationProvider
    {
        public string Name => "Inventory";

        public async Task Migrate(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(connectionString);
            var db = new InventoryContext(optionsBuilder.Options);

            await db.Database.MigrateAsync();
        }
    }
}
