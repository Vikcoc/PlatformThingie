using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PlatformInterfaces;
using RabbitMQ.Client;
using System.Data;

namespace UserPermissionConsumer
{
    public class ComponentDefinition : IPlatformComponentDefinition
    {
        public string GivenName => "UserPermission";

        public void AddRoutes(IEndpointRouteBuilder endpoints, IConfiguration config)
        {
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddKeyedSingleton<IConnection>("UserPermission", (ser, key) =>
            {
                var config = ser.GetRequiredService<IConfiguration>();
                var factory = new ConnectionFactory
                {
                    HostName = config["Rabbit:Host"],
                    Port = int.Parse(config["Rabbit:Port"]!)
                };
                return factory.CreateConnection();
            });
            services.AddKeyedSingleton<IDbConnection>("UserPermission", (db, key) =>
            {
                var theConnection = db.GetRequiredService<IConfiguration>()["ConnectionStrings:UserPermission"];
                return new NpgsqlConnection(theConnection);
            });
            services.AddSingleton<PUserPermissionRepo>();
            services.AddHostedService<RPermissionsConsumer>();
        }
    }
}
