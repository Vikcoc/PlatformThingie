using Dependencies;
using InvTemplateInfo.functionalities.permission.repo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace InvTemplateInfo.functionalities.permission
{
    public static class PermissionController
    {
        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/invtemplate/permission/all", GetPermission)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, "AuthAdmin"));
            endpoints.MapPost("/invtemplate/permission", CreatePermission)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, "AuthAdmin"));
            endpoints.MapDelete("/invtemplate/permission", DeletePermission)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, "AuthAdmin"));
        }

        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<PPermissionRepo>();
        }

        public static async Task<IResult> GetPermission([FromServices] PPermissionRepo permissionRepo)
        {
            return TypedResults.Ok(await permissionRepo.GetPermissions());
        }

        public static async Task<IResult> DeletePermission([FromBody] string permission, [FromServices] PPermissionRepo permissionRepo)
        {
            var existing = await permissionRepo.GetTemplatesWithAttributesByPermission(permission);
            if(existing.Any())
                return TypedResults.BadRequest(existing);
            await permissionRepo.DeletePermission(permission);
            return TypedResults.NoContent();
        }

        public static async Task<IResult> CreatePermission([FromBody] string permission, [FromServices] PPermissionRepo permissionRepo)
        {
            if (await permissionRepo.PermissionExists(permission))
                return TypedResults.BadRequest("Already existing");
            await permissionRepo.AddPermission(permission);
            return TypedResults.Ok();
        }
    }
}
