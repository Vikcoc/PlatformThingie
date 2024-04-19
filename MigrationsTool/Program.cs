using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MigrationsTool;
using PlatformInterfaces;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.Sources.Add(new JsonConfigurationSource()
{
    Path = "conf/config.json"
});

builder.Services.AddSingleton<ICollection<IMigrationProvider>>(x =>
[
    new UsersDbComponent.seeding.Seeder(),
    new InventoryDbComponent.seeding.Seeder(),
    new InvTemplateDbComponent.seeding.Seeder()
]);

using IHost host = builder.Build();

var providers = host.Services.GetRequiredService<ICollection<IMigrationProvider>>();
var configuration = host.Services.GetRequiredService<IConfiguration>();

var configs = configuration.GetSection("Connections").Get<ConfigDbDto[]>();
if (configs == null)
    return;

foreach (var config in configs)
    foreach (var provider in providers.Where(x => x.Name == config.ProviderName))
        await provider.Migrate(config.ConnectionString);