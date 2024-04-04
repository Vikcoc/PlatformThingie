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

        public void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            ReadInventoryController.AddRoutes(endpoints);

            endpoints.MapGet("/inventory/test",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "tests", "test.html"), "text/html"));
            endpoints.MapGet("/inventory/test/style",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "tests", "test.css"), "text/css"));
            endpoints.MapGet("/inventory/test/js",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "tests", "test.js"), "text/javascript"));
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
