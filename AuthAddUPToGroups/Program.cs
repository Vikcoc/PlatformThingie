using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UsersDbComponent.entities;
using UsersDbComponent.seeding;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.Sources.Add(new JsonConfigurationSource()
{
    Path = "conf/config.json"
});


using IHost host = builder.Build();

var configuration = host.Services.GetRequiredService<IConfiguration>();

var connection = configuration["ConnectionString"];
var groups = configuration.GetSection("Conf").Get<ConfElement[]>();

var optionsBuilder = new DbContextOptionsBuilder();
optionsBuilder.UseNpgsql(connection);
var db = new AuthContext(optionsBuilder.Options);

//only existing users to existing groups because ef can't unexisting users with new claims to existing group

var permissionsToAdd = groups!.SelectMany(x => x.Permissions).Except(db.AuthPermissions.Select(x => x.AuthPermissionName));
db.AuthPermissions.AddRange(permissionsToAdd.Select(x => new AuthPermission { AuthPermissionName = x }));

foreach (var group in groups!)
{
    var grPrToAdd = group.Permissions.Except(db.AuthGroupPermissions.Where(x => x.AuthGroupName == group.Group).Select(x => x.AuthPermissionName));
    db.AuthGroupPermissions.AddRange(grPrToAdd.Select(x => new AuthGroupPermission
    {
        AuthGroupName = group.Group,
        AuthPermissionName = x,
        AuthGroup = null!,
        AuthPermission = null!
    }));

    var usToAdd = group.Users.Except(db.AuthUserGroups.Where(x => x.AuthGroupName == group.Group).SelectMany(x => x.AuthUser.AuthUserClaims.Where(x => x.AuthClaimName == SeedAuthClaimNames.Email).Select(x => x.AuthClaimValue)));
    db.AuthUserGroups.AddRange(usToAdd.Select(x => new AuthUserGroup
    {
        AuthGroup = null!,
        AuthUser = null!,
        AuthGroupName = group.Group,
        AuthUserId = db.AuthUsers.First(y => y.AuthUserClaims.Any(z => z.AuthClaimName == SeedAuthClaimNames.Email && z.AuthClaimValue == x)).AuthUserId
    }));
}

await db.SaveChangesAsync();


struct ConfElement
{
    public string Group { get; set; }
    public string[] Users { get; set; }
    public string[] Permissions { get; set; }
}