using AuthFrontend.seeds;
using Dependencies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using UserInfo.functionalities.user.dtos;
using UserInfo.functionalities.user.Repositories;
using UsersDbComponent;

namespace UserInfo.functionalities.user
{
    internal static class UserController
    {
        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/profile/info", GetUserProps)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access));

            endpoints.MapGet("/profile/claims", GetClaims)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access));

            endpoints.MapPost("/profile/info", UpdateEditableClaims)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access));

            endpoints.MapGet("/user/all", GetUsersWithGroups)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, UserStrings.AuthAdmin));

            endpoints.MapGet("/user/groups", GetGroups)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, UserStrings.AuthAdmin));

            endpoints.MapPost("/user", SaveUser)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, UserStrings.AuthAdmin));

            endpoints.MapGet("/user/group/all", GetGroupsWithPermissions)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, UserStrings.AuthAdmin));

            endpoints.MapGet("/user/group/permissions", GetPermissions)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, UserStrings.AuthAdmin));

            endpoints.MapPost("/user/group", CreateOrSaveGroup)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, UserStrings.AuthAdmin));
            endpoints.MapDelete("/user/group", DeleteGroup)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, UserStrings.AuthAdmin));
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

        public static async Task<IResult> GetUsersWithGroups([FromServices] PProfileRepo profileRepo)
        {
            return TypedResults.Ok(await profileRepo.GetUsersWithEmailAndGroups());
        }
        public static async Task<IResult> GetGroups([FromServices] PProfileRepo profileRepo)
        {
            return TypedResults.Ok(await profileRepo.GetGroups());
        }

        public static async Task<IResult> SaveUser([FromBody]UserWithGroupDto user, [FromServices] PProfileRepo profileRepo)
        {
            await profileRepo.DeleteAndRewriteUserGroups(user.UserId, user.Groups);
            return TypedResults.NoContent();
        }

        public static async Task<IResult> GetGroupsWithPermissions([FromServices] PProfileRepo profileRepo)
        {
            return TypedResults.Ok(await profileRepo.GetGroupsWithPermissions());
        }

        public static async Task<IResult> GetPermissions([FromServices] PProfileRepo profileRepo)
        {
            return TypedResults.Ok(await profileRepo.GetPermissions());
        }

        public static async Task<IResult> CreateOrSaveGroup([FromBody] GroupWithPermissionsDto group, [FromServices] PProfileRepo profileRepo)
        {
            if (await profileRepo.GroupExists(group.GroupName))
            {
                await profileRepo.DeleteAndRewriteGroupPermissions(group.GroupName, group.Permissions);
                return TypedResults.NoContent();
            }
            var success = await profileRepo.CreateGroupPermissions(group.GroupName, group.Permissions);
            if(success) 
                return TypedResults.NoContent();
            return TypedResults.Problem("Cannot create");
        }

        public static async Task<IResult> DeleteGroup([FromBody] string groupName, [FromServices] PProfileRepo profileRepo)
        {
            if(!await profileRepo.GroupExists(groupName))
                return TypedResults.BadRequest("Wrong group");
            var success = await profileRepo.DeleteGroup(groupName);
            if (success)
                return TypedResults.NoContent();
            return TypedResults.Problem("Cannot delete");
        }
    }
}
