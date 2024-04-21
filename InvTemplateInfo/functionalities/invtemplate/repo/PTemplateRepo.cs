using Dapper;
using InvTemplateDbComponent.entities;
using InvTemplateInfo.functionalities.invtemplate.dtos;
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
                SELECT t."{nameof(InvTemplate.InvTemplateName)}" as TemplateName,
                t."{nameof(InvTemplate.InvTemplateVersion)}" as TemplateVersion,
                t."{nameof(InvTemplate.Released)}" as Released,
                t."{nameof(InvTemplate.InvTemplateVersion)}" = (SELECT MAX(tt."{nameof(InvTemplate.InvTemplateVersion)}") FROM "{nameof(InvTemplateContext.InvTemplates)}" tt WHERE t."{nameof(InvTemplate.InvTemplateName)}" = tt."{nameof(InvTemplate.InvTemplateName)}") as Latest
                FROM "{nameof(InvTemplateContext.InvTemplates)}" t;
                
                SELECT a."{nameof(InvTemplateAttr.InvTemplateName)}" as TemplateName,
                a."{nameof(InvTemplateAttr.InvTemplateVersion)}" as TemplateVersion,
                a."{nameof(InvTemplateAttr.InvTemplateAttrName)}" as AttrName,
                a."{nameof(InvTemplateAttr.InvTemplateAttrValue)}" as AttrValue,
                a."{nameof(InvTemplateAttr.InvTemplateAttrAction)}" as AttrAction
                FROM "{nameof(InvTemplateContext.InvTemplatesAttrs)}" a;

                SELECT ap."{nameof(InvTemplateAttrPermission.InvTemplateName)}" as TemplateName,
                ap."{nameof(InvTemplateAttrPermission.InvTemplateVersion)}" as TemplateVersion,
                ap."{nameof(InvTemplateAttrPermission.InvTemplateAttrName)}" as AttrName,
                ap."{nameof(InvTemplateAttrPermission.InvTemplatePermissionName)}" as PermissionName
                FROM "{nameof(InvTemplateContext.InvTemplatesAttrPermissions)}" ap;

                SELECT ea."{nameof(InvTemplateEntAttr.InvTemplateName)}" as TemplateName,
                ea."{nameof(InvTemplateEntAttr.InvTemplateVersion)}" as TemplateVersion,
                ea."{nameof(InvTemplateEntAttr.InvTemplateEntAttrName)}" as AttrName,
                ea."{nameof(InvTemplateEntAttr.InvTemplateEntAttrAction)}" as AttrAction
                FROM "{nameof(InvTemplateContext.InvTemplateEntAttrs)}" ea;

                SELECT eap."{nameof(InvTemplateEntAttrPermission.InvTemplateName)}" as TemplateName,
                eap."{nameof(InvTemplateEntAttrPermission.InvTemplateVersion)}" as TemplateVersion,
                eap."{nameof(InvTemplateEntAttrPermission.InvTemplateEntAttrName)}" as AttrName,
                eap."{nameof(InvTemplateEntAttrPermission.InvTemplatePermissionName)}" as PermissionName,
                eap."{nameof(InvTemplateEntAttrPermission.Writeable)}" as Writeable
                FROM "{nameof(InvTemplateContext.InvTemplateEntAttrPermissions)}" eap;
                """;

            var res = await _dbConnection.QueryMultipleAsync(query);

            var temp = await res.ReadAsync<(string TemplateName, int TemplateVersion, bool Released, bool Latest)>();
            var attr = await res.ReadAsync<(string TemplateName, int TemplateVersion, string AttrName, string AttrValue, string AttrAction)>();
            var attrp = await res.ReadAsync<(string TemplateName, int TemplateVersion, string AttrName, string PermissionName)>();
            var eattr = await res.ReadAsync<(string TemplateName, int TemplateVersion, string AttrName, string AttrAction)>();
            var eattrp = await res.ReadAsync<(string TemplateName, int TemplateVersion, string AttrName, string PermissionName, bool Writeable)>();

            var grp = temp.GroupJoin(attr.GroupJoin(attrp,
                        x => new { x.TemplateName, x.TemplateVersion, x.AttrName },
                        x => new { x.TemplateName, x.TemplateVersion, x.AttrName },
                        (a, b) => new
                        {
                            a.TemplateName,
                            a.TemplateVersion,
                            a.AttrName,
                            a.AttrAction,
                            a.AttrValue,
                            Permissions = b.Select(x => x.PermissionName).ToArray()
                        }),
                    x => new { x.TemplateName, x.TemplateVersion },
                    x => new { x.TemplateName, x.TemplateVersion },
                    (a, b) => new
                    {
                        a.TemplateName,
                        a.TemplateVersion,
                        a.Released,
                        a.Latest,
                        TemplateAttributes = b.Select(x => new AttrWithPerm
                        {
                            AttrName = x.AttrName,
                            AttrValue = x.AttrValue,
                            AttrAction = x.AttrAction,
                            Permissions = x.Permissions
                        }).ToArray()
                    }
                )
                .Join(
                temp.GroupJoin(eattr.GroupJoin(eattrp,
                            x => new { x.TemplateName, x.TemplateVersion, x.AttrName },
                            x => new { x.TemplateName, x.TemplateVersion, x.AttrName },
                            (a, b) => new
                            {
                                a.TemplateName,
                                a.TemplateVersion,
                                a.AttrName,
                                a.AttrAction,
                                Permissions = b.Select(x => new PermissionWithWrite
                                {
                                    Permission = x.PermissionName,
                                    Writeable = x.Writeable
                                }).ToArray()
                            }),
                        x => new { x.TemplateName, x.TemplateVersion },
                        x => new { x.TemplateName, x.TemplateVersion },
                        (a, b) => new
                        {
                            a.TemplateName,
                            a.TemplateVersion,
                            a.Released,
                            a.Latest,
                            EntityAttributes = b.Select(x => new EntAttrWithPerm
                            {
                                AttrName = x.AttrName,
                                AttrAction = x.AttrAction,
                                Permissions = x.Permissions
                            }).ToArray()
                        }
                    ),
                    x => new { x.TemplateName, x.TemplateVersion },
                    x => new { x.TemplateName, x.TemplateVersion },
                    (a, b) => new TemplateWithAttrAndPerm
                    {
                        TemplateName = a.TemplateName,
                        TemplateVersion = a.TemplateVersion,
                        Released = a.Released,
                        Latest = a.Latest,
                        EntityAttributes = b.EntityAttributes,
                        TemplateAttributes = a.TemplateAttributes
                    }
                ).ToArray();
            return grp;
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
