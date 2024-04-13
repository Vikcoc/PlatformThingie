using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatformInterfaces;

namespace InventoryScripts
{
    public class ComponentDefinition : IPlatformComponentDefinition
    {
        public string GivenName => "Inventory";

        public void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/inventory/actions/someaction",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "scripts", "someaction.js"), "text/javascript"));
            endpoints.MapGet("/inventory/actions/someaction2",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "scripts", "someaction2.js"), "text/javascript"));
            endpoints.MapGet("/inventory/actions/displayheader",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "scripts", "displayheader.js"), "text/javascript"));
        }

        public void AddServices(IServiceCollection services)
        {
        }
    }
}
