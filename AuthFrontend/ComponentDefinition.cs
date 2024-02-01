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
            endpoints.MapGet("/bundle", 
                () => 
                Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "bundle.js"), "text/javascript")); 
        }

        public void AddServices(IServiceCollection services)
        {
        }
    }
}
