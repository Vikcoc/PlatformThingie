using Dapper;
using InvTemplateDbComponent.entities;
using InvTemplateInfo.functionalities.invtemplate.dtos;
using InvTemplateInfo.functionalities.permission.dtos;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace InvTemplateInfo.functionalities.invtemplate.repo
{
    public class PTemplateRepo([FromKeyedServices("InvTemplate")] IDbConnection dbConnection)
    {
        private readonly IDbConnection _dbConnection = dbConnection;

        public async Task<TemplateWithAttrAndPerm[]> GetTemplates()
        {
            var query = $"""
                SELECT a."{nameof(InvTemplateAttr.InvTemplateName)}" as TemplateName,
                a."{nameof(InvTemplateAttr.InvTemplateVersion)}" as TemplateVersion,
                a."{nameof(InvTemplateAttr.InvTemplateAttrName)}" as AttrName,
                a."{nameof(InvTemplateAttr.InvTemplateAttrValue)}" as AttrValue,
                a."{nameof(InvTemplateAttr.InvTemplateAttrAction)}" as AttrAction,
                ap."{nameof(InvTemplateAttrPermission.InvTemplatePermission)}" as AttrPerm;
                FROM "{nameof(InvTemplateContext.InvTemplatesAttrs)}" a 
                LEFT JOIN "{nameof(InvTemplateContext.InvTemplatesAttrPermissions)}" ap
                ON a."{nameof(InvTemplateAttr.InvTemplateName)}" = ap."{nameof(InvTemplateAttrPermission.InvTemplateName)}"
                AND a."{nameof(InvTemplateAttr.InvTemplateVersion)}" = ap."{nameof(InvTemplateAttrPermission.InvTemplateVersion)}"
                AND a."{nameof(InvTemplateAttr.InvTemplateAttrName)}" = ap."{nameof(InvTemplateAttrPermission.InvTemplateAttrName)}";

                SELECT ea."{nameof(InvTemplateEntAttr.InvTemplateName)}" as TemplateName,
                ea."{nameof(InvTemplateEntAttr.InvTemplateVersion)}" as TemplateVersion,
                ea."{nameof(InvTemplateEntAttr.InvTemplateEntAttrName)}" as AttrName,
                ea."{nameof(InvTemplateEntAttr.InvTemplateEntAttrAction)}" as AttrAction,
                eap."{nameof(InvTemplateEntAttrPermission.InvTemplatePermissionName)}" as AttrPerm,
                eap."{nameof(InvTemplateEntAttrPermission.Writeable)}" as AttrWrite;
                FROM "{nameof(InvTemplateContext.InvTemplateEntAttrs)}" a
                LEFT JOIN "{nameof(InvTemplateContext.InvTemplateEntAttrPermissions)}" eap
                ON a."{nameof(InvTemplateEntAttr.InvTemplateName)}" = eap."{nameof(InvTemplateEntAttrPermission.InvTemplateName)}"
                AND a."{nameof(InvTemplateEntAttr.InvTemplateVersion)}" = eap."{nameof(InvTemplateEntAttrPermission.InvTemplateVersion)}"
                AND a."{nameof(InvTemplateEntAttr.InvTemplateEntAttrName)}" = eap."{nameof(InvTemplateEntAttrPermission.InvTemplateEntAttrName)}";
                """;

            var res = await _dbConnection.QueryMultipleAsync(query);

            var attr = await res.ReadAsync<(string TemplateName, int TemplateVersion, string AttrName, string AttrValue, string AttrAction, string AttrPerm)>();

            var entAttr = await res.ReadAsync<(string TemplateName, int TemplateVersion, string AttrName, string AttrAction, string AttrPerm, bool AttrWrite)>();

            var attrGroup = attr.GroupBy(x => new
            {
                x.TemplateName,
                x.TemplateVersion,
                x.AttrName,
                x.AttrValue,
                x.AttrAction
            }).GroupBy(x => new
            {
                x.Key.TemplateName,
                x.Key.TemplateVersion
            });

            var entAttrGroup = entAttr.GroupBy(x => new
            {
                x.TemplateName,
                x.TemplateVersion,
                x.AttrName,
                x.AttrAction
            }).GroupBy(x => new
            {
                x.Key.TemplateName,
                x.Key.TemplateVersion
            });

            var grp = attrGroup.Join(entAttrGroup,
                x => x.Key,
                x => x.Key,
                (a, b) => new TemplateWithAttrAndPerm
                {
                    TemplateName = a.Key.TemplateName,
                    TemplateVersion = a.Key.TemplateVersion,
                    TemplateAttributes = a.Select(x => new AttrWithPerm
                    {
                        AttrName = x.Key.AttrName,
                        AttrValue = x.Key.AttrValue,
                        AttrAction = x.Key.AttrAction,
                        Permissions = x.Select(y => y.AttrPerm).ToArray()
                    }).ToArray(),
                    EntityAttributes = b.Select(x => new EntAttrWithPerm
                    {
                        AttrName = x.Key.AttrName,
                        AttrAction = x.Key.AttrAction,
                        Permissions = x.Select(y => new PermissionWithWrite
                        {
                            Permission = y.AttrPerm,
                            Writeable = y.AttrWrite
                        }).ToArray()
                    }).ToArray(),
                }).ToList();

            grp.AddRange(attrGroup.Where(
                x => !grp.Where(y => y.TemplateName == x.Key.TemplateName && y.TemplateVersion == x.Key.TemplateVersion).Any())
                .Select(x => new TemplateWithAttrAndPerm
                {
                    TemplateName = x.Key.TemplateName,
                    TemplateVersion = x.Key.TemplateVersion,
                    TemplateAttributes = x.Select(x => new AttrWithPerm
                    {
                        AttrName = x.Key.AttrName,
                        AttrValue = x.Key.AttrValue,
                        AttrAction= x.Key.AttrAction,
                        Permissions = x.Select(y => y.AttrPerm).ToArray()
                    }).ToArray(),
                    EntityAttributes = []
                }));
            grp.AddRange(entAttrGroup.Where(
                x => !grp.Where(y => y.TemplateName == x.Key.TemplateName && y.TemplateVersion == x.Key.TemplateVersion).Any())
                .Select(x => new TemplateWithAttrAndPerm
                {
                    TemplateName = x.Key.TemplateName,
                    TemplateVersion = x.Key.TemplateVersion,
                    TemplateAttributes = [],
                    EntityAttributes = x.Select(x => new EntAttrWithPerm
                    {
                        AttrName = x.Key.AttrName,
                        AttrAction = x.Key.AttrAction,
                        Permissions = x.Select(y => new PermissionWithWrite
                        {
                            Permission = y.AttrPerm,
                            Writeable = y.AttrWrite
                        }).ToArray()
                    }).ToArray()
                }));


            return [.. grp];
        }

        public async Task<string[]> GetPermissions()
        {
            var query = $"""
                SELECT "{nameof(InvTemplatePermission.InvTemplatePermissionName)}" as Value
                FROM "{nameof(InvTemplateContext.InvTemplatePermissions)}";
                """;

            var res = await _dbConnection.QueryAsync<string>(query);
            return res.ToArray();
        }
    }
}
