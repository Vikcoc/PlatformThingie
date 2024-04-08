using Dependencies;
using InventoryInfo.functionalities.readingInventory.DTOs;
using InventoryInfo.functionalities.readingInventory.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Security;

namespace InventoryInfo.functionalities.readingInventory
{
    internal static class ReadInventoryController
    {
        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/inventory/all/filtered", GetFiltered)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access));
        }

        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<PInventoryRepo>();
        }

        public async static Task<IResult> GetFiltered([FromBody] InventoryPropertyFilter filter, HttpContext context, [FromServices] PInventoryRepo repo)
        {
            var permissions = context.User.Claims.Where(x => x.Type == ImportantStrings.PermissionSet).Select(x => x.Value).ToArray();

            if(!permissions.Any())
                return TypedResults.BadRequest("No permissions");

            if (filter.EntityProperties.Length == 0 || filter.TemplateProperties.Length == 0)
                return TypedResults.BadRequest("No properties");

            var res = await repo.GetEntities(permissions, filter.EntityProperties, filter.TemplateProperties);

            return TypedResults.Ok(res);
        }

    }
}
