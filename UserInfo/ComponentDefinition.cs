using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatformInterfaces;

namespace UserInfo
{
    public class ComponentDefinition : IPlatformComponentDefinition
    {
        public string GivenName => "User";

        public void AddServices(IServiceCollection services)
        {
        }

        public void AddRoutes(IEndpointRouteBuilder endpoints)
        {

        }
    }
}
