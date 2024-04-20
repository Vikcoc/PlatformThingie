using InvTemplateInfo.functionalities.permission;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PlatformInterfaces;
using System.Data;

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

            endpoints.MapGet("/invtemplate",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "invtemplate", "template.html"), "text/html"));
            endpoints.MapGet("/invtemplate/style",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "invtemplate", "template.css"), "text/css"));
            endpoints.MapGet("/invtemplate/script",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "invtemplate", "template.js"), "text/javascript"));

            endpoints.MapGet("/invtemplate/permission",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "permission", "permission.html"), "text/html"));
            endpoints.MapGet("/invtemplate/permission/script",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "permission", "permission.js"), "text/javascript"));

            PermissionController.AddRoutes(endpoints);

        }

        public void AddServices(IServiceCollection services)
        {
            services.AddKeyedScoped<IDbConnection>("InvTemplate", (db, key) =>
            {
                var theConnection = db.GetRequiredService<IConfiguration>()["ConnectionStrings:InvTemplate"];
                return new NpgsqlConnection(theConnection);
            });

            PermissionController.AddServices(services);
        }
    }
}
