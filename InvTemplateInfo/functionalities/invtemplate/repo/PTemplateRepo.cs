﻿using Dapper;
using InvTemplateDbComponent.entities;
using InvTemplateInfo.functionalities.invtemplate.dtos;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Text.RegularExpressions;

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

        public async Task<bool> ExistsTemplate(string templateName, int templateVersion)
        {
            var query = $"""
                SELECT COUNT(1) as Value
                FROM "{nameof(InvTemplateContext.InvTemplates)}"
                WHERE "{nameof(InvTemplate.InvTemplateName)}" = @TemplateName
                AND "{nameof(InvTemplate.InvTemplateVersion)}" = @TemplateVersion;
                """;

            var res = await _dbConnection.QueryFirstAsync<int>(query, new
            {
                TemplateName = templateName,
                TemplateVersion = templateVersion
            });
            return 0 != res;
        }

        public async Task CreateTemplate(TemplateForCreationDto dto)
        {
            var tempQuery = $"""
                INSERT INTO "{nameof(InvTemplateContext.InvTemplates)}"
                ("{nameof(InvTemplate.InvTemplateName)}", "{nameof(InvTemplate.InvTemplateVersion)}", "{nameof(InvTemplate.Released)}")
                VALUES (@TemplateName, @TemplateVersion, false);
                """;
            var tempAttrQuery = $"""
                INSERT INTO "{nameof(InvTemplateContext.InvTemplatesAttrs)}"
                ("{nameof(InvTemplateAttr.InvTemplateName)}", "{nameof(InvTemplateAttr.InvTemplateVersion)}", "{nameof(InvTemplateAttr.InvTemplateAttrName)}", "{nameof(InvTemplateAttr.InvTemplateAttrAction)}", "{nameof(InvTemplateAttr.InvTemplateAttrValue)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrAction, @AttrValue);
                """;

            var tempAttrPermQuery = $"""
                INSERT INTO "{nameof(InvTemplateContext.InvTemplatesAttrPermissions)}"
                ("{nameof(InvTemplateAttrPermission.InvTemplateName)}", "{nameof(InvTemplateAttrPermission.InvTemplateVersion)}", "{nameof(InvTemplateAttrPermission.InvTemplateAttrName)}", "{nameof(InvTemplateAttrPermission.InvTemplatePermissionName)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrPerm);
                """;

            var entAttrQuery = $"""
                INSERT INTO "{nameof(InvTemplateContext.InvTemplateEntAttrs)}"
                ("{nameof(InvTemplateEntAttr.InvTemplateName)}", "{nameof(InvTemplateEntAttr.InvTemplateVersion)}", "{nameof(InvTemplateEntAttr.InvTemplateEntAttrName)}", "{nameof(InvTemplateEntAttr.InvTemplateEntAttrAction)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrAction);
                """;

            var entAttrPermQuery = $"""
                INSERT INTO "{nameof(InvTemplateContext.InvTemplateEntAttrPermissions)}"
                ("{nameof(InvTemplateEntAttrPermission.InvTemplateName)}", "{nameof(InvTemplateEntAttrPermission.InvTemplateVersion)}", "{nameof(InvTemplateEntAttrPermission.InvTemplateEntAttrName)}", "{nameof(InvTemplateEntAttrPermission.InvTemplatePermissionName)}", "{nameof(InvTemplateEntAttrPermission.Writeable)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrPerm, @Writeable);
                """;

            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();

            await _dbConnection.ExecuteAsync(tempQuery, new
            {
                TemplateName = dto.TemplateName,
                TemplateVersion = dto.TemplateVersion
            });

            await _dbConnection.ExecuteAsync(tempAttrQuery, dto.TemplateAttributes.Select(x => new
            {
                TemplateName = dto.TemplateName,
                TemplateVersion = dto.TemplateVersion,
                AttrName = x.AttrName,
                AttrAction = x.AttrAction,
                AttrValue = x.AttrValue,
            }).ToArray());
            await _dbConnection.ExecuteAsync(tempAttrPermQuery, dto.TemplateAttributes.SelectMany(x => x.Permissions.Select(y => new
            {
                TemplateName = dto.TemplateName,
                TemplateVersion = dto.TemplateVersion,
                AttrName = x.AttrName,
                AttrPerm = y
            })).ToArray());

            await _dbConnection.ExecuteAsync(entAttrQuery, dto.EntityAttributes.Select(x => new
            {
                TemplateName = dto.TemplateName,
                TemplateVersion = dto.TemplateVersion,
                AttrName = x.AttrName,
                AttrAction = x.AttrAction
            }).ToArray());
            await _dbConnection.ExecuteAsync(entAttrPermQuery, dto.EntityAttributes.SelectMany(x => x.Permissions.Select(y => new
            {
                TemplateName = dto.TemplateName,
                TemplateVersion = dto.TemplateVersion,
                AttrName = x.AttrName,
                AttrPerm = y.Permission,
                Writeable = y.Writeable
            })).ToArray());

            transaction.Commit();
        }

        public async Task DeleteAndRecreateParams(TemplateForCreationDto dto)
        {
            var tempQuery = $"""
                DELETE FROM "{nameof(InvTemplateContext.InvTemplateEntAttrPermissions)}"
                WHERE "{nameof(InvTemplateEntAttrPermission.InvTemplateName)}" = @TemplateName AND "{nameof(InvTemplateEntAttrPermission.InvTemplateVersion)}" = @TemplateVersion;

                DELETE FROM "{nameof(InvTemplateContext.InvTemplateEntAttrs)}"
                WHERE "{nameof(InvTemplateEntAttr.InvTemplateName)}" = @TemplateName AND "{nameof(InvTemplateEntAttr.InvTemplateVersion)}" = @TemplateVersion;

                DELETE FROM "{nameof(InvTemplateContext.InvTemplatesAttrPermissions)}"
                WHERE "{nameof(InvTemplateAttrPermission.InvTemplateName)}" = @TemplateName AND "{nameof(InvTemplateAttrPermission.InvTemplateVersion)}" = @TemplateVersion;

                DELETE FROM "{nameof(InvTemplateContext.InvTemplatesAttrs)}"
                WHERE "{nameof(InvTemplateAttr.InvTemplateName)}" = @TemplateName AND "{nameof(InvTemplateAttr.InvTemplateVersion)}" = @TemplateVersion;
                """;
            var tempAttrQuery = $"""
                INSERT INTO "{nameof(InvTemplateContext.InvTemplatesAttrs)}"
                ("{nameof(InvTemplateAttr.InvTemplateName)}", "{nameof(InvTemplateAttr.InvTemplateVersion)}", "{nameof(InvTemplateAttr.InvTemplateAttrName)}", "{nameof(InvTemplateAttr.InvTemplateAttrAction)}", "{nameof(InvTemplateAttr.InvTemplateAttrValue)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrAction, @AttrValue);
                """;

            var tempAttrPermQuery = $"""
                INSERT INTO "{nameof(InvTemplateContext.InvTemplatesAttrPermissions)}"
                ("{nameof(InvTemplateAttrPermission.InvTemplateName)}", "{nameof(InvTemplateAttrPermission.InvTemplateVersion)}", "{nameof(InvTemplateAttrPermission.InvTemplateAttrName)}", "{nameof(InvTemplateAttrPermission.InvTemplatePermissionName)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrPerm);
                """;

            var entAttrQuery = $"""
                INSERT INTO "{nameof(InvTemplateContext.InvTemplateEntAttrs)}"
                ("{nameof(InvTemplateEntAttr.InvTemplateName)}", "{nameof(InvTemplateEntAttr.InvTemplateVersion)}", "{nameof(InvTemplateEntAttr.InvTemplateEntAttrName)}", "{nameof(InvTemplateEntAttr.InvTemplateEntAttrAction)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrAction);
                """;

            var entAttrPermQuery = $"""
                INSERT INTO "{nameof(InvTemplateContext.InvTemplateEntAttrPermissions)}"
                ("{nameof(InvTemplateEntAttrPermission.InvTemplateName)}", "{nameof(InvTemplateEntAttrPermission.InvTemplateVersion)}", "{nameof(InvTemplateEntAttrPermission.InvTemplateEntAttrName)}", "{nameof(InvTemplateEntAttrPermission.InvTemplatePermissionName)}", "{nameof(InvTemplateEntAttrPermission.Writeable)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrPerm, @Writeable);
                """;

            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();

            await _dbConnection.ExecuteAsync(tempQuery, new
            {
                TemplateName = dto.TemplateName,
                TemplateVersion = dto.TemplateVersion
            });

            await _dbConnection.ExecuteAsync(tempAttrQuery, dto.TemplateAttributes.Select(x => new
            {
                TemplateName = dto.TemplateName,
                TemplateVersion = dto.TemplateVersion,
                AttrName = x.AttrName,
                AttrAction = x.AttrAction,
                AttrValue = x.AttrValue,
            }).ToArray());
            await _dbConnection.ExecuteAsync(tempAttrPermQuery, dto.TemplateAttributes.SelectMany(x => x.Permissions.Select(y => new
            {
                TemplateName = dto.TemplateName,
                TemplateVersion = dto.TemplateVersion,
                AttrName = x.AttrName,
                AttrPerm = y
            })).ToArray());

            await _dbConnection.ExecuteAsync(entAttrQuery, dto.EntityAttributes.Select(x => new
            {
                TemplateName = dto.TemplateName,
                TemplateVersion = dto.TemplateVersion,
                AttrName = x.AttrName,
                AttrAction = x.AttrAction
            }).ToArray());
            await _dbConnection.ExecuteAsync(entAttrPermQuery, dto.EntityAttributes.SelectMany(x => x.Permissions.Select(y => new
            {
                TemplateName = dto.TemplateName,
                TemplateVersion = dto.TemplateVersion,
                AttrName = x.AttrName,
                AttrPerm = y.Permission,
                Writeable = y.Writeable
            })).ToArray());

            transaction.Commit();
        }

        public async Task ReleaseTemplate(TemplateForReleaseDto dto)
        {
            var tempQuery = $"""
                UPDATE "{nameof(InvTemplateContext.InvTemplates)}"
                SET "{nameof(InvTemplate.Released)}" = true
                WHERE "{nameof(InvTemplate.InvTemplateName)}" = @TemplateName
                AND "{nameof(InvTemplate.InvTemplateVersion)}" = @TemplateVersion;
                """;

            await _dbConnection.ExecuteAsync(tempQuery, new
            {
                TemplateName = dto.TemplateName,
                TemplateVersion = dto.TemplateVersion
            });
        }

        public async Task<ReleaseTemplateDto?> GetTemplate(string templateName, int templateVersion)
        {
            var query = $"""
                SELECT tp."{nameof(InvTemplate.InvTemplateName)}" as TemplateName,
                tp."{nameof(InvTemplate.InvTemplateVersion)}" as TemplateVersion
                FROM "{nameof(InvTemplateContext.InvTemplates)}" tp
                WHERE "{nameof(InvTemplate.InvTemplateName)}" = @TemplateName
                AND "{nameof(InvTemplate.InvTemplateVersion)}" = @TemplateVersion;

                SELECT a."{nameof(InvTemplateAttr.InvTemplateName)}" as TemplateName,
                a."{nameof(InvTemplateAttr.InvTemplateVersion)}" as TemplateVersion,
                a."{nameof(InvTemplateAttr.InvTemplateAttrName)}" as AttrName,
                a."{nameof(InvTemplateAttr.InvTemplateAttrValue)}" as AttrValue,
                a."{nameof(InvTemplateAttr.InvTemplateAttrAction)}" as AttrAction,
                ap."{nameof(InvTemplateAttrPermission.InvTemplatePermissionName)}" as PermissionName
                FROM "{nameof(InvTemplateContext.InvTemplatesAttrs)}" a
                LEFT JOIN "{nameof(InvTemplateContext.InvTemplatesAttrPermissions)}" ap
                ON ap."{nameof(InvTemplateAttrPermission.InvTemplateName)}" = a."{nameof(InvTemplateAttr.InvTemplateName)}"
                AND ap."{nameof(InvTemplateAttrPermission.InvTemplateVersion)}" = a."{nameof(InvTemplateAttr.InvTemplateVersion)}"
                AND ap."{nameof(InvTemplateAttrPermission.InvTemplateAttrName)}" = a."{nameof(InvTemplateAttr.InvTemplateAttrName)}"
                WHERE a."{nameof(InvTemplateAttr.InvTemplateName)}" = @TemplateName
                AND a."{nameof(InvTemplateAttr.InvTemplateVersion)}" = @TemplateVersion;

                SELECT ea."{nameof(InvTemplateEntAttr.InvTemplateName)}" as TemplateName,
                ea."{nameof(InvTemplateEntAttr.InvTemplateVersion)}" as TemplateVersion,
                ea."{nameof(InvTemplateEntAttr.InvTemplateEntAttrName)}" as AttrName,
                ea."{nameof(InvTemplateEntAttr.InvTemplateEntAttrAction)}" as AttrAction,
                eap."{nameof(InvTemplateEntAttrPermission.InvTemplatePermissionName)}" as PermissionName,
                eap."{nameof(InvTemplateEntAttrPermission.Writeable)}" as Writeable
                FROM "{nameof(InvTemplateContext.InvTemplateEntAttrs)}" ea
                LEFT JOIN "{nameof(InvTemplateContext.InvTemplateEntAttrPermissions)}" eap
                ON eap."{nameof(InvTemplateEntAttrPermission.InvTemplateName)}" = ea."{nameof(InvTemplateEntAttr.InvTemplateName)}"
                AND eap."{nameof(InvTemplateEntAttrPermission.InvTemplateVersion)}" = ea."{nameof(InvTemplateEntAttr.InvTemplateVersion)}"
                AND eap."{nameof(InvTemplateEntAttrPermission.InvTemplateEntAttrName)}" = ea."{nameof(InvTemplateEntAttr.InvTemplateEntAttrName)}"
                WHERE ea."{nameof(InvTemplateEntAttr.InvTemplateName)}" = @TemplateName
                AND ea."{nameof(InvTemplateEntAttr.InvTemplateVersion)}" = @TemplateVersion;
                """;

            var res = await _dbConnection.QueryMultipleAsync(query, new
            {
                TemplateName = templateName,
                TemplateVersion = templateVersion
            });

            var tp = await res.ReadFirstOrDefaultAsync<(string TemplateName, int TemplateVersion)>();
            var attr = await res.ReadAsync<(string TemplateName, int TemplateVersion, string AttrName, string AttrValue, string AttrAction, string PermissionName)>();
            var eattr = await res.ReadAsync<(string TemplateName, int TemplateVersion, string AttrName, string AttrAction, string PermissionName, bool Writeable)>();

            if (tp == default)
                return null;

            var ret = new ReleaseTemplateDto { 
                TemplateName = templateName,
                TemplateVersion = templateVersion,
                TemplateAttributes = attr.GroupBy(x => new {x.TemplateName, x.TemplateVersion, x.AttrName, x.AttrValue, x.AttrAction})
                .Select(x => new AttrWithPerm
                {
                    AttrName = x.Key.AttrName,
                    AttrAction = x.Key.AttrAction,
                    AttrValue = x.Key.AttrValue,
                    Permissions = x.Select(y => y.PermissionName).ToArray()
                }).ToArray(),
                EntityAttributes = eattr.GroupBy(x => new {x.TemplateName, x.TemplateVersion, x.AttrName, x.AttrAction})
                .Select(x => new EntAttrWithPerm
                {
                    AttrName = x.Key.AttrName,
                    AttrAction = x.Key.AttrAction,
                    Permissions = x.Select(y => new PermissionWithWrite
                    {
                        Permission = y.PermissionName,
                        Writeable = y.Writeable
                    }).ToArray()
                }).ToArray()
            };

            return ret;
        }
    }
}
