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
            endpoints.MapGet("/style",
                () =>
                Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "style.css"), "text/css"));
            endpoints.MapGet("/favicon.ico",
                () =>
                Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "modsig.svg"), "image/svg+xml"));
        }

        public void AddServices(IServiceCollection services)
        {
        }
    }
}
