using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using PlatformInterfaces;

namespace InvTemplateInfo
{
    public class ComponentDefinition : IPlatformComponentDefinition
    {
        public string GivenName => "InvTemplate";

        public void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            //design
            //4 vertical columns (similar to the 2 of user admin)
            //1 for group (one of each version)
            //1 for template attributes
            //1 for entity attributes
            //1 for the access rights (with writeable check that gets hidden for template attributes)
        }

        public void AddServices(IServiceCollection services)
        {
            
        }
    }
}
