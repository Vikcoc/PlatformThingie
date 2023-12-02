using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace PlatformInterfaces;
public interface IPlatformComponentDefinition
{
    public string GivenName {get;}
    void AddServices(IServiceCollection services);
    void AddRoutes(IEndpointRouteBuilder endpoints);
}