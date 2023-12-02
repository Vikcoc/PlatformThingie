using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatformInterfaces;

namespace DefaultsAndStuff;
public class ComponentDefinition : IPlatformComponentDefinition
{
    public string GivenName => "Yess";

    public void AddServices(IServiceCollection services)
    { }

    public void AddRoutes(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/woo", () => "This is woo");
    }
}
