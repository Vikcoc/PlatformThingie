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
            endpoints.MapGet("/public/color",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "color.css"), "text/css"));
            endpoints.MapGet("/public/font",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "font.css"), "text/css"));
            endpoints.MapGet("/favicon.ico",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "modsig.svg"), "image/svg+xml"));
            endpoints.MapGet("/public/style",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "style.css"), "text/css"));
            endpoints.MapGet("/public/topbar",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "topbar.css"), "text/css"));
            endpoints.MapGet("/public/filled-tonal-icon-button",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "filled-tonal-icon-button.js"), "text/javascript"));
            endpoints.MapGet("/public/filled-tonal-button",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "filled-tonal-button.js"), "text/javascript"));
            endpoints.MapGet("/public/back-logo",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "back-logo.svg"), "image/svg+xml"));

            //the index i guess
            endpoints.MapGet("/", () => Results.Redirect("/about"));

            //the about page
            endpoints.MapGet("/about",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "about", "about.html"), "text/html"));
            
        }

        public void AddServices(IServiceCollection services)
        {
        }
    }
}
