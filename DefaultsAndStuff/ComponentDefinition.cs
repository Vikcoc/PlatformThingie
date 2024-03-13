using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatformInterfaces;

namespace DefaultsAndStuff;
public class ComponentDefinition : IPlatformComponentDefinition
{
    public string GivenName => "Yess";

    public void AddServices(IServiceCollection services)
    {
        services.AddScoped<SampleService>();
    }

    public void AddRoutes(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/woo", () => "This is woo");
        endpoints.MapGet("/clamp", SomeInjectTest);
    }

    internal string SomeInjectTest([FromQuery] int number, [FromServices] SampleService service)
    {
        return $"Clamped number is {service.ModuloToInterval(number, -5, 5)}";
    }
}
