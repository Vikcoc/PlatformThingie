using AuthFrontend.functionalities.loggingIn;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            LogInController.AddRoutes(endpoints);


            //the index i guess
            endpoints.MapGet("/", () => Results.Redirect("/about"));
            
            //the page with sign in
            endpoints.MapGet("/about", () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "about", "index.html"), "text/html"));
            endpoints.MapGet("/signIn.js",
                () =>
                Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "about", "signIn.js"), "text/javascript"));
            endpoints.MapGet("/style",
                () =>
                Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "about", "style.css"), "text/css"));

            endpoints.MapPost("/about/token", ([FromBody] string Token) =>
            {
                Console.WriteLine(Token);
                return "This is a test string for communication";
            });

            //general look
            endpoints.MapGet("/color",
                () =>
                Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "color.css"), "text/css"));
            endpoints.MapGet("/font",
                () =>
                Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "font.css"), "text/css"));
            endpoints.MapGet("/favicon.ico",
                () =>
                Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "all", "modsig.svg"), "image/svg+xml"));
        }

        public void AddServices(IServiceCollection services)
        {
            LogInController.AddServices(services);
        }
    }
}
