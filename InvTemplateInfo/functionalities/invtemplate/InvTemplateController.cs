using Dependencies;
using InvTemplateInfo.functionalities.invtemplate.dtos;
using InvTemplateInfo.functionalities.invtemplate.repo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace InvTemplateInfo.functionalities.invtemplate
{
    public static class InvTemplateController
    {
        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<PTemplateRepo>();
        }

        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/invtemplate/all", GetTemplates)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, TemplatesStrings.TemplateAdmin));

            endpoints.MapGet("/invtemplate/action", GetActions)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, TemplatesStrings.TemplateAdmin));
            endpoints.MapGet("/invtemplate/allpermissions", GetPermissions)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, TemplatesStrings.TemplateAdmin));
            
            endpoints.MapPost("/invtemplate", SaveTemplate)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, TemplatesStrings.TemplateAdmin));
            endpoints.MapPost("/invtemplate/release", ReleaseTemplate)
                .RequireAuthorization(p => p.RequireClaim(ImportantStrings.Purpose, ImportantStrings.Access)
                                            .RequireClaim(ImportantStrings.PermissionSet, TemplatesStrings.TemplateAdmin));

        }

        public static async Task<IResult> GetTemplates([FromServices] PTemplateRepo templateRepo)
        {
            return TypedResults.Ok(await templateRepo.GetTemplates());
        }

        public static IResult GetActions()
        {
            var files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "scripts"));
            
            return TypedResults.Ok(files.Select(x => "/inventory/actions/" + Path.GetFileNameWithoutExtension(x)).ToArray());
        }

        public static async Task<IResult> GetPermissions([FromServices] PTemplateRepo templateRepo)
        {
            return TypedResults.Ok(await templateRepo.GetPermissions());
        }

        public static async Task<IResult> SaveTemplate([FromBody] TemplateForCreationDto dto, [FromServices] PTemplateRepo templateRepo)
        {
            if (await templateRepo.ExistsTemplate(dto.TemplateName, dto.TemplateVersion))
                await templateRepo.DeleteAndRecreateParams(dto);
            else
                await templateRepo.CreateTemplate(dto);
            return TypedResults.NoContent();
        }

        public static async Task<IResult> ReleaseTemplate([FromBody] TemplateForReleaseDto dto, [FromServices] PTemplateRepo templateRepo)
        {
            if (await templateRepo.ExistsTemplate(dto.TemplateName, dto.TemplateVersion))
            {
                await templateRepo.ReleaseTemplate(dto);
                return TypedResults.NoContent();
            }
            return TypedResults.BadRequest("Bad template");
        }

    }
}
