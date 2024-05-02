using Dependencies;
using InventoryInfo.functionalities.readingInventory.DTOs;
using InventoryInfo.functionalities.readingInventory.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryInfo.functionalities.readingInventory
{
    internal static class ReadInventoryController
    {
        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/inventory/filtered", GetFiltered)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access));

            endpoints.MapGet("/inventory/attributes", GetAttributes)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access));
            endpoints.MapGet("/inventory/entity-attributes", GetEntityAttributes)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access));


            endpoints.MapGet("/inventory/templates", GetTemplates)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, InventoryStrings.InventoryAdmin));
            endpoints.MapPost("/inventory", MakeEntity)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, InventoryStrings.InventoryAdmin));
        }

        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<PInventoryRepo>();
        }

        public async static Task<IResult> GetFiltered([FromBody] InventoryPropertyFilter filter, HttpContext context, [FromServices] PInventoryRepo repo)
        {
            var permissions = context.User.Claims.Where(x => x.Type == ImportantStrings.PermissionSet).Select(x => x.Value).ToArray();

            if(permissions.Length == 0)
                return TypedResults.BadRequest("No permissions");

            if (filter.EntityProperties.Length == 0 || filter.TemplateProperties.Length == 0)
                return TypedResults.BadRequest("No properties");

            var res = await repo.GetEntities(permissions, filter.EntityProperties, filter.TemplateProperties);

            return TypedResults.Ok(res);
        }

        public async static Task<IResult> GetAttributes(HttpContext context, [FromServices] PInventoryRepo repo)
        {
            var permissions = context.User.Claims.Where(x => x.Type == ImportantStrings.PermissionSet).Select(x => x.Value).ToArray();

            if (permissions.Length == 0)
                return TypedResults.BadRequest("No permissions");

            var res = await repo.GetAttributes(permissions);

            return TypedResults.Ok(res);
        }

        public async static Task<IResult> GetEntityAttributes(HttpContext context, [FromServices] PInventoryRepo repo)
        {
            var permissions = context.User.Claims.Where(x => x.Type == ImportantStrings.PermissionSet).Select(x => x.Value).ToArray();

            if (permissions.Length == 0)
                return TypedResults.BadRequest("No permissions");

            var res = await repo.GetEntityAttributes(permissions);

            return TypedResults.Ok(res);
        }

        public async static Task<IResult> GetTemplates([FromServices] PInventoryRepo repo)
        {
            var res = await repo.GetLatestTemplates();

            return TypedResults.Ok(res);
        }

        public async static Task<IResult> MakeEntity(HttpContext context, [FromBody] InventoryCreateEntityDto entity,[FromServices] PInventoryRepo repo)
        {
            var existingProps = await repo.GetEntityPropertiesOfTemplate(entity.TemplateName, entity.TemplateVersion);

            if (entity.EntityProperties.Select(x => x.Name).Distinct().Count()
                    != entity.EntityProperties.Length
                || existingProps.Except(entity.EntityProperties.Select(x => x.Name)).Any()
                || entity.EntityProperties.Select(x => x.Name).Except(existingProps).Any())
                return TypedResults.BadRequest("Mismatch of properties");

            var res = await repo.CreateEntity(entity);

            if (!res.HasValue)
                return TypedResults.Problem("Cannot create entity");

            return TypedResults.Ok(res);
        }
    }
}
