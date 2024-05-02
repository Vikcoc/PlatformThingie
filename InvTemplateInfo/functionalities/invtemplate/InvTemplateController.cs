using Dependencies;
using InvTemplateInfo.functionalities.invtemplate.dtos;
using InvTemplateInfo.functionalities.invtemplate.repo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace InvTemplateInfo.functionalities.invtemplate
{
    public static class InvTemplateController
    {
        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<PTemplateRepo>();
            services.AddKeyedSingleton<IConnection>("InvTemplate", (ser, key) =>
            {
                var config = ser.GetRequiredService<IConfiguration>();
                var factory = new ConnectionFactory
                {
                    HostName = config["Rabbit:Host"],
                    Port = int.Parse(config["Rabbit:Port"]!)
                };
                return factory.CreateConnection();
            });
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

        public static IResult GetActions([FromServices] IConfiguration configuration)
        {
            var files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "scripts"));
            
            return TypedResults.Ok(files.Select(x => configuration["ScriptsRoute"] + Path.GetFileNameWithoutExtension(x)).ToArray());
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

        public static async Task<IResult> ReleaseTemplate([FromBody] TemplateForReleaseDto dto
            , [FromServices] PTemplateRepo templateRepo
            , [FromKeyedServices("InvTemplate")] IConnection connection)
        {
            //does not make sense to export template without attributes and permissions on attributes
            var template = await templateRepo.GetTemplate(dto.TemplateName, dto.TemplateVersion);
            if (!template.HasValue 
                || template.Value.TemplateAttributes.Length == 0
                || template.Value.TemplateAttributes.Any(x => x.Permissions.Length == 0)
                || template.Value.EntityAttributes.Length == 0
                || template.Value.EntityAttributes.Any(x => x.Permissions.Length == 0))
                return TypedResults.BadRequest("Bad template");
            
            await templateRepo.ReleaseTemplate(dto);
            
            //template
            var message = await Task.Run(() => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(template)));
            using var channel = connection.CreateModel();
            await Task.Run(() => channel.QueueDeclare(queue: TemplatesStrings.TemplateRelease,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null));
            await Task.Run(() => channel.BasicPublish(exchange: string.Empty,
                     routingKey: TemplatesStrings.TemplateRelease,
                     basicProperties: null,
                     body: message));

            //permissions
            var perm = await Task.Run(() =>
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(
            template.Value.TemplateAttributes.SelectMany(x => x.Permissions)
            .Concat(template.Value.EntityAttributes.SelectMany(x => x.Permissions.Select(x => x.Permission)))
            .Distinct()
            .Where(x => x != null)
            )));
            using var permissionChannel = connection.CreateModel();
            await Task.Run(() => channel.QueueDeclare(queue: TemplatesStrings.PermissionRelease,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null));
            await Task.Run(() => channel.BasicPublish(exchange: string.Empty,
                     routingKey: TemplatesStrings.PermissionRelease,
                     basicProperties: null,
                     body: perm));

            return TypedResults.NoContent();
        }

    }
}
