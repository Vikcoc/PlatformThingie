
using Microsoft.Extensions.Configuration;
using PlatformInterfaces;

namespace MigrationsTool
{
    internal class MigratorService(ICollection<IMigrationProvider> providers, IConfiguration configuration) : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly ICollection<IMigrationProvider> _providers = providers;
        private readonly IConfiguration _configuration = configuration;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var configs = _configuration.GetSection("Connections").Get<ConfigDbDto[]>();
            if (configs == null)
                return;

            foreach(var config in configs)
                foreach (var provider in _providers.Where(x => x.Name == config.ProviderName))
                    await provider.Migrate(config.ConnectionString);
        }
    }
}
