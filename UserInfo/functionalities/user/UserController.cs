using AuthFrontend.seeds;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;
using System.Reflection;
using UserInfo.functionalities.user.Repositories;

namespace UserInfo.functionalities.user
{
    internal static class UserController
    {
        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/profile/info", GetUserProps)
                .RequireAuthorization(p => p.RequireClaim("Purpose", "Access"));
        }

        public static void AddServices(IServiceCollection services)
        {
            services.AddKeyedScoped<IDbConnection>("User", (db, key) =>
            {
                var theConnection = db.GetRequiredService<IConfiguration>()["ConnectionStrings:User"];
                return new NpgsqlConnection(theConnection);
            });
            services.AddScoped<PProfileRepo>();
        }

        public static async Task<IResult> GetUserProps(HttpContext context, [FromServices] PProfileRepo profileRepo)
        {
            var userId = Guid.Parse(context.User.Claims.First(x => x.Type == "UserId").Value);
            var claims = await profileRepo.GetUserClaimsByUserId(userId);
            return TypedResults.Ok(claims.Select(x => new
            {
                x.AuthClaimName,
                x.AuthClaimValue,
                x.AuthClaimRight
            }));
        }
    }
}
