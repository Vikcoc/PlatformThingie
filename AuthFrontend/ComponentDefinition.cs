using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatformInterfaces;

namespace AuthFrontend
{
    public class ComponentDefinition : IPlatformComponentDefinition
    {
        public string GivenName => "Login with Google";

        public void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/", () => {
                //return Directory.GetCurrentDirectory();
                return Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "index.html"), "text/html");
            });
        }

        public void AddServices(IServiceCollection services)
        {
        }
    }
}
