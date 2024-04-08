using AuthFrontend.entities;
using AuthFrontend.seeds;
using Microsoft.EntityFrameworkCore;
using PlatformInterfaces;
using UsersDbComponent.entities;

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

            var claims = new SeedAuthClaimNames();

            var toAdd = claims.Except(db.AuthClaims.Select(x => new KeyValuePair<string, AuthClaimRights>(x.AuthClaimName,x.AuthClaimRight)));
            var toEdit = toAdd.Join(db.AuthClaims, x => x.Key, y => y.AuthClaimName, (x, y) => (x, y));
            toAdd = toAdd.Except(toEdit.Select(a => a.x));

            db.AuthClaims.AddRange(toAdd.Select(x => new AuthClaim
            {
                AuthClaimName = x.Key,
                AuthClaimRight = x.Value
            }));

            foreach(var (x, y) in toEdit) 
            {
                y.AuthClaimRight = x.Value;
            }

            if (!await db.AuthPermissions.AnyAsync(x => x.AuthPermissionName == "Admin"))
            {
                var permission = new AuthPermission { AuthPermissionName = "Admin" };
                var group = new AuthGroup { AuthGroupName = "Admin" };
                var groupPermission = new AuthGroupPermission
                {
                    AuthGroupName = group.AuthGroupName,
                    AuthGroup = group,
                    AuthPermission = permission,
                    AuthPermissionName = permission.AuthPermissionName
                };
                db.AuthGroups.Add(group);
                db.AuthPermissions.Add(permission);
                db.AuthGroupPermissions.Add(groupPermission);
            }

            await db.SaveChangesAsync();
        }
    }
}
