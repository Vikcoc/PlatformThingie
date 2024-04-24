using InvTemplateDbComponent.entities;
using Microsoft.EntityFrameworkCore;
using PlatformInterfaces;

namespace InvTemplateDbComponent.seeding
{
    public class Seeder : IMigrationProvider
    {
        public string Name => "InvTemplate";

        public async Task Migrate(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(connectionString);
            var db = new InvTemplateContext(optionsBuilder.Options);

            await db.Database.MigrateAsync();
        }
    }
}
