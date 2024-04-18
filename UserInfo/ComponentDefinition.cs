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
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "profile", "profile.html"), "text/html"));
            endpoints.MapGet("/profile/style",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "profile", "profile.css"), "text/css"));
            endpoints.MapGet("/profile/script",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "profile", "profile.js"), "text/javascript"));

            endpoints.MapGet("/user",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "user", "user.html"), "text/html"));
            endpoints.MapGet("/user/style",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "user", "user.css"), "text/css"));
            endpoints.MapGet("/user/script",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "user", "user.js"), "text/javascript"));
            endpoints.MapGet("/user/group",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "group", "group.html"), "text/html"));
            endpoints.MapGet("/user/group/style",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "group", "group.css"), "text/css"));
            endpoints.MapGet("/user/group/script",
                () => Results.File(Path.Combine(Directory.GetCurrentDirectory(), "pages", "group", "group.js"), "text/javascript"));

            UserController.AddRoutes(endpoints);
        }
    }
}
