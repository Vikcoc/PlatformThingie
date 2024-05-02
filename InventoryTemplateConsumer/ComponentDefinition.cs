using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PlatformInterfaces;
using RabbitMQ.Client;
using System.Data;

namespace InventoryTemplateConsumer
{
    public class ComponentDefinition : IPlatformComponentDefinition
    {
        public string GivenName => "InventoryTemplate";

        public void AddRoutes(IEndpointRouteBuilder endpoints, IConfiguration config)
        {
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddKeyedSingleton<IConnection>(GivenName, (ser, key) =>
            {
                var config = ser.GetRequiredService<IConfiguration>();
                var factory = new ConnectionFactory
                {
                    HostName = config["Rabbit:Host"],
                    Port = int.Parse(config["Rabbit:Port"]!)
                };
                return factory.CreateConnection();
            });
            services.AddKeyedSingleton<IDbConnection>(GivenName, (db, key) =>
            {
                var theConnection = db.GetRequiredService<IConfiguration>()["ConnectionStrings:InventoryTemplate"];
                return new NpgsqlConnection(theConnection);
            });
            services.AddSingleton<PInventoryTemplateRepo>();
            services.AddHostedService<RInventoryTemplateConsumer>();
        }
    }
}
