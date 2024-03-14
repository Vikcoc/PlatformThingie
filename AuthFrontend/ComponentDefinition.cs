using AuthFrontend.functionalities.loggingIn;
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
            LogInController.AddRoutes(endpoints);

            endpoints.MapGet("/login/test", () => "It worked")
                .RequireAuthorization(p => p.RequireClaim("Purpose", "Access"));

            //the index i guess
            endpoints.MapGet("/", () => Results.Redirect("/about"));

            
        }

        public void AddServices(IServiceCollection services)
        {
            LogInController.AddServices(services);
        }
    }
}
