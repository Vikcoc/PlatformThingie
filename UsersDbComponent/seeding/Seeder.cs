using AuthFrontend.entities;
using AuthFrontend.seeds;
using Microsoft.EntityFrameworkCore;
using PlatformInterfaces;

namespace UsersDbComponent.seeding
{
    public class Seeder : IMigrationProvider
    {
        public string Name => "User";

        public async Task Migrate(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(connectionString);
            var db = new AuthContext(optionsBuilder.Options);

            await db.Database.MigrateAsync();

            var claims = new[] { SeedAuthClaimNames.Email, SeedAuthClaimNames.Username };

            var toAdd = claims.Except(db.AuthClaims.Select(x => x.AuthClaimName));

            db.AuthClaims.AddRange(toAdd.Select(x => new AuthClaim
            {
                AuthClaimName = x
            }));

            await db.SaveChangesAsync();
        }
    }
}
