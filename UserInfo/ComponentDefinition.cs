using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using PlatformInterfaces;
using System.Data;
using UserInfo.functionalities.user;
using UserInfo.functionalities.user.Repositories;

namespace UserInfo
{
    public class ComponentDefinition : IPlatformComponentDefinition
    {
        public string GivenName => "User";

        public void AddServices(IServiceCollection services)
        {
            services.AddKeyedScoped<IDbConnection>("User", (db, key) =>
            {
                var theConnection = db.GetRequiredService<IConfiguration>()["ConnectionStrings:User"];
                return new NpgsqlConnection(theConnection);
            });
            services.AddScoped<PProfileRepo>();

            UserController.AddServices(services);
        }

        public void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/profile",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "user", "profile.html"), "text/html"));
            endpoints.MapGet("/profile/style",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "user", "profile.css"), "text/css"));
            endpoints.MapGet("/profile/profile",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "user", "profile.js"), "text/javascript"));

            UserController.AddRoutes(endpoints);
        }
    }
}
