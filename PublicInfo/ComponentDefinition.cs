using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatformInterfaces;

namespace PublicInfo
{
    public class ComponentDefinition : IPlatformComponentDefinition
    {
        public string GivenName => "Public info";

        public void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            //general stuff
            endpoints.MapGet("/color",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "color.css"), "text/css"));
            endpoints.MapGet("/font",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "font.css"), "text/css"));
            endpoints.MapGet("/favicon.ico",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "modsig.svg"), "image/svg+xml"));
            endpoints.MapGet("/style",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "style.css"), "text/css"));

            //the about page
            endpoints.MapGet("/about",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "about", "index.html"), "text/html"));
            
        }

        public void AddServices(IServiceCollection services)
        {
        }
    }
}
