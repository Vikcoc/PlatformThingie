using AuthFrontend.seeds;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using UserInfo.functionalities.user.dtos;
using UserInfo.functionalities.user.Repositories;

namespace UserInfo.functionalities.user
{
    internal static class UserController
    {
        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/profile/info", GetUserProps)
                .RequireAuthorization(p => p.RequireClaim("purpose", "access"));

            endpoints.MapGet("/profile/claims", GetClaims)
                .RequireAuthorization(p => p.RequireClaim("purpose", "access"));

            endpoints.MapPost("/profile/info", UpdateEditableClaims)
                .RequireAuthorization(p => p.RequireClaim("purpose", "access"));
        }

        public static void AddServices(IServiceCollection services)
        {
            
        }

        public static async Task<IResult> GetUserProps(HttpContext context, [FromServices] PProfileRepo profileRepo)
        {
            var userId = Guid.Parse(context.User.Claims.First(x => x.Type == SeedAuthClaimNames.UserId).Value);
            var claims = await profileRepo.GetUserClaimsByUserId(userId);
            return TypedResults.Ok(claims.Select(x => new
            {
                x.AuthClaimName,
                x.AuthClaimValue,
                x.AuthClaimRight
            }));
        }

        public static async Task<IResult> GetClaims([FromServices] PProfileRepo profileRepo)
            => TypedResults.Ok(await profileRepo.GetEditableClaims());

        public static async Task<IResult> UpdateEditableClaims(HttpContext context, [FromBody] List<UserClaimDto> claims , [FromServices] PProfileRepo profileRepo)
        {
            var available = await profileRepo.GetEditableClaims();

            if (claims.Select(x => x.AuthClaimName).ToHashSet().Except(available).Any())
                return TypedResults.BadRequest("Cannot edit undefined claims");

            var userId = Guid.Parse(context.User.Claims.First(x => x.Type == SeedAuthClaimNames.UserId).Value);

            await profileRepo.DeleteAndRewriteEditableClaims(userId, claims);
            
            return TypedResults.NoContent();
        }
    }
}
