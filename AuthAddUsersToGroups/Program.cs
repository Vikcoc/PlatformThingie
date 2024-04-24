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

var connection = configuration["Conf:ConnectionString"];
var group = configuration["Conf:Group"];
var users = configuration.GetSection("Conf:List").Get<string[]>();
if (connection == null || group == null || users == null)
    return;

var optionsBuilder = new DbContextOptionsBuilder();
optionsBuilder.UseNpgsql(connection);
var db = new AuthContext(optionsBuilder.Options);

db.AuthUserGroups.AddRange(users.Select(
    usr => db.AuthUsers.FirstOrDefault(
        us => us.AuthUserClaims.Any(
            uc => uc.AuthClaimName == SeedAuthClaimNames.Email && uc.AuthClaimValue == usr)))
    .Where(us => us != null)
    .Select( us => new UsersDbComponent.entities.AuthUserGroup()
    {
        AuthGroup = null!,
        AuthGroupName = group,
        AuthUser = us!,
        AuthUserId = us!.AuthUserId
    }));

await db.SaveChangesAsync();