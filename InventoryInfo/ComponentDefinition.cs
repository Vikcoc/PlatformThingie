using InventoryInfo.functionalities.readingInventory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PlatformInterfaces;
using System.Data;

namespace InventoryInfo
{
    public class ComponentDefinition : IPlatformComponentDefinition
    {
        public string GivenName => "Inventory";

        public void AddRoutes(IEndpointRouteBuilder endpoints, IConfiguration config)
        {
            ReadInventoryController.AddRoutes(endpoints);

            endpoints.MapGet("/inventory",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "inventory", "inventory.html"), "text/html"));
            endpoints.MapGet("/inventory/script",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "inventory", "inventory.js"), "text/javascript"));
            endpoints.MapGet("/inventory/template",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "template", "template.html"), "text/html"));
            endpoints.MapGet("/inventory/template/script",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "template", "template.js"), "text/javascript"));
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddKeyedScoped<IDbConnection>("Inventory", (db, key) =>
            {
                var theConnection = db.GetRequiredService<IConfiguration>()["ConnectionStrings:Inventory"];
                return new NpgsqlConnection(theConnection);
            });

            ReadInventoryController.AddServices(services);
        }
    }
}
