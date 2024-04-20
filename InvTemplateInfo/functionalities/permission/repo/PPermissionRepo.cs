using Dapper;
using InvTemplateDbComponent.entities;
using InvTemplateInfo.functionalities.permission.dtos;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace InvTemplateInfo.functionalities.permission.repo
{
    public class PPermissionRepo([FromKeyedServices("InvTemplate")] IDbConnection dbConnection)
    {
        private readonly IDbConnection _dbConnection = dbConnection;

        public async Task<PermissionDto[]> GetPermissions()
        {
            var query = $"""
                SELECT p."{nameof(InvTemplatePermission.InvTemplatePermissionName)}" as PermissionName,
                COUNT(ap."{nameof(InvTemplateAttrPermission.InvTemplatePermissionName)}") + COUNT(eap."{nameof(InvTemplateEntAttrPermission.InvTemplatePermissionName)}") as ConnectedCount
                FROM "{nameof(InvTemplateContext.InvTemplatePermissions)}" p
                LEFT JOIN "{nameof(InvTemplateContext.InvTemplatesAttrPermissions)}" ap
                ON p."{nameof(InvTemplatePermission.InvTemplatePermissionName)}" = ap."{nameof(InvTemplateAttrPermission.InvTemplatePermissionName)}"
                LEFT JOIN "{nameof(InvTemplateContext.InvTemplateEntAttrPermissions)}" eap
                ON p."{nameof(InvTemplatePermission.InvTemplatePermissionName)}" = eap."{nameof(InvTemplateEntAttrPermission.InvTemplatePermissionName)}"
                GROUP BY p."{nameof(InvTemplatePermission.InvTemplatePermissionName)}";
                """;

            var res = await _dbConnection.QueryAsync<(string PermissionName, int ConnectedCount)>(query);



            return res.Select(x => new PermissionDto
            {
                PermissionName = x.PermissionName,
                CanDelete = x.ConnectedCount == 0
            }).ToArray();
        }

        public async Task<TemplateWithAttributesDto[]> GetTemplatesWithAttributesByPermission(string permission)
        {
            var query = $"""
                SELECT a."{nameof(InvTemplateAttr.InvTemplateAttrName)}" as Attribute, a."{nameof(InvTemplateAttr.InvTemplateName)}" as Template, a."{nameof(InvTemplateAttr.InvTemplateVersion)}" as Version
                FROM "{nameof(InvTemplateContext.InvTemplatesAttrs)}" a
                JOIN "{nameof(InvTemplateContext.InvTemplatesAttrPermissions)}" p
                ON a."{nameof(InvTemplateAttr.InvTemplateAttrName)}" = p."{nameof(InvTemplateAttrPermission.InvTemplateAttrName)}"
                AND a."{nameof(InvTemplateAttr.InvTemplateName)}" = p."{nameof(InvTemplateAttrPermission.InvTemplateName)}"
                AND a."{nameof(InvTemplateAttr.InvTemplateVersion)}" = p."{nameof(InvTemplateAttrPermission.InvTemplateVersion)}"
                WHERE p."{nameof(InvTemplateAttrPermission.InvTemplatePermissionName)}" = @Permission;

                SELECT a."{nameof(InvTemplateEntAttr.InvTemplateEntAttrName)}" as Attribute, a."{nameof(InvTemplateEntAttr.InvTemplateName)}" as Template, a."{nameof(InvTemplateEntAttr.InvTemplateVersion)}" as Version
                FROM "{nameof(InvTemplateContext.InvTemplateEntAttrs)}" a
                JOIN "{nameof(InvTemplateContext.InvTemplateEntAttrPermissions)}" p
                ON a."{nameof(InvTemplateEntAttr.InvTemplateEntAttrName)}" = p."{nameof(InvTemplateEntAttrPermission.InvTemplateEntAttrName)}"
                AND a."{nameof(InvTemplateEntAttr.InvTemplateName)}" = p."{nameof(InvTemplateEntAttrPermission.InvTemplateName)}"
                AND a."{nameof(InvTemplateEntAttr.InvTemplateVersion)}" = p."{nameof(InvTemplateEntAttrPermission.InvTemplateVersion)}"
                WHERE p."{nameof(InvTemplateAttrPermission.InvTemplatePermissionName)}" = @Permission;
                """;

            var res = await _dbConnection.QueryMultipleAsync(query, new
            {
                Permission = permission
            });
            var entityProps = await res.ReadAsync<(string Template, int Version, string Attribute)>();
            var templateProps = await res.ReadAsync<(string Template, int Version, string EntityAttribute)>();

            var dtos = entityProps.GroupBy(x => new { x.Template, x.Version})
                .Join(templateProps.GroupBy(x => new { x.Template, x.Version })
                , a => a.Key
                , b => b.Key
                , (a, b) => new TemplateWithAttributesDto
                {
                    TemplateName = a.Key.Template,
                    TemplateVersion = a.Key.Version,
                    Attributes = a.Select(x => x.Attribute).ToArray(),
                    EntityAttributes = b.Select(x => x.EntityAttribute).ToArray()
                }).ToArray();

            return dtos;
        }

        public async Task DeletePermission(string permission)
        {
            var query = $"""
                DELETE FROM "{nameof(InvTemplateContext.InvTemplatePermissions)}"
                WHERE "{nameof(InvTemplatePermission.InvTemplatePermissionName)}" = @Permission;
                """;

            await _dbConnection.ExecuteAsync(query, new
            {
                Permission = permission
            });
        }

        public async Task AddPermission(string permission)
        {
            var query = $"""
                INSERT INTO "{nameof(InvTemplateContext.InvTemplatePermissions)}"
                ("{nameof(InvTemplatePermission.InvTemplatePermissionName)}")
                VALUES (@Permission);
                """;

            await _dbConnection.ExecuteAsync(query, new
            {
                Permission = permission
            });
        }

        public async Task<bool> PermissionExists(string permission)
        {
            var query = $"""
                Select COUNT(1) as Value
                FROM "{nameof(InvTemplateContext.InvTemplatePermissions)}"
                WHERE "{nameof(InvTemplatePermission.InvTemplatePermissionName)}" = (@Permission);
                """;

            return 0 < await _dbConnection.QueryFirstAsync<int>(query, new
            {
                Permission = permission
            });
        }
    }
}
