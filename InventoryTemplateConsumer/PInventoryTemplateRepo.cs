using Dapper;
using InventoryDbComponent.entities;
using InventoryTemplateConsumer.dtos;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace InventoryTemplateConsumer
{
    public class PInventoryTemplateRepo([FromKeyedServices("InventoryTemplate")]IDbConnection dbConnection)
    {
        private readonly IDbConnection _dbConnection = dbConnection;

        public async Task<bool> ExistsTemplate(string templateName, int templateVersion)
        {
            var query = $"""
                SELECT COUNT(1) as Value
                FROM "{nameof(InventoryContext.InventoryTemplates)}"
                WHERE "{nameof(InventoryTemplate.InventoryTemplateName)}" = @TemplateName
                AND "{nameof(InventoryTemplate.InventoryTemplateVersion)}" = @TemplateVersion;
                """;
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();

            var res = await _dbConnection.QueryFirstAsync<int>(query, new
            {
                TemplateName = templateName,
                TemplateVersion = templateVersion
            });
            
            transaction.Rollback();
            return 0 != res;
        }

        public async Task CreateTemplate(ReleaseTemplateDto dto)
        {
            var tempQuery = $"""
                INSERT INTO "{nameof(InventoryContext.InventoryTemplates)}"
                ("{nameof(InventoryTemplate.InventoryTemplateName)}", "{nameof(InventoryTemplate.InventoryTemplateVersion)}")
                VALUES (@TemplateName, @TemplateVersion);
                """;
            var tempAttrQuery = $"""
                INSERT INTO "{nameof(InventoryContext.InventoryTemplateAttributes)}"
                ("{nameof(InventoryTemplateAttribute.InventoryTemplateName)}", "{nameof(InventoryTemplateAttribute.InventoryTemplateVersion)}", "{nameof(InventoryTemplateAttribute.InventoryTemplateAttributeName)}", "{nameof(InventoryTemplateAttribute.InventoryTemplateAttributeAction)}", "{nameof(InventoryTemplateAttribute.InventoryTemplateAttributeValue)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrAction, @AttrValue);
                """;

            var tempAttrPermQuery = $"""
                INSERT INTO "{nameof(InventoryContext.InventoryTemplateAttributeReads)}"
                ("{nameof(InventoryTemplateAttributeRead.InventoryTemplateName)}", "{nameof(InventoryTemplateAttributeRead.InventoryTemplateVersion)}", "{nameof(InventoryTemplateAttributeRead.InventoryTemplateAttributeName)}", "{nameof(InventoryTemplateAttributeRead.Permission)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrPerm);
                """;

            var entAttrQuery = $"""
                INSERT INTO "{nameof(InventoryContext.InventoryTemplateEntityAttributes)}"
                ("{nameof(InventoryTemplateEntityAttribute.InventoryTemplateName)}", "{nameof(InventoryTemplateEntityAttribute.InventoryTemplateVersion)}", "{nameof(InventoryTemplateEntityAttribute.InventoryTemplateEntityAttributeName)}", "{nameof(InventoryTemplateEntityAttribute.InventoryTemplateEntityAttributeAction)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrAction);
                """;

            var entAttrPermQuery = $"""
                INSERT INTO "{nameof(InventoryContext.InventoryTemplateEntityAttributesPermissions)}"
                ("{nameof(InventoryTemplateEntityAttributePermission.InventoryTemplateName)}", "{nameof(InventoryTemplateEntityAttributePermission.InventoryTemplateVersion)}", "{nameof(InventoryTemplateEntityAttributePermission.InventoryTemplateEntityAttributeName)}", "{nameof(InventoryTemplateEntityAttributePermission.Permission)}", "{nameof(InventoryTemplateEntityAttributePermission.Writeable)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrPerm, @Writeable);
                """;

            if(_dbConnection.State != ConnectionState.Open)
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

        public async Task DeleteAndRecreateParams(ReleaseTemplateDto dto)
        {
            var tempQuery = $"""
                DELETE FROM "{nameof(InventoryContext.InventoryTemplateEntityAttributesPermissions)}"
                WHERE "{nameof(InventoryTemplateEntityAttributePermission.InventoryTemplateName)}" = @TemplateName AND "{nameof(InventoryTemplateEntityAttributePermission.InventoryTemplateVersion)}" = @TemplateVersion;

                DELETE FROM "{nameof(InventoryContext.InventoryTemplateEntityAttributes)}"
                WHERE "{nameof(InventoryTemplateEntityAttribute.InventoryTemplateName)}" = @TemplateName AND "{nameof(InventoryTemplateEntityAttribute.InventoryTemplateVersion)}" = @TemplateVersion;

                DELETE FROM "{nameof(InventoryContext.InventoryTemplateAttributeReads)}"
                WHERE "{nameof(InventoryTemplateAttributeRead.InventoryTemplateName)}" = @TemplateName AND "{nameof(InventoryTemplateAttributeRead.InventoryTemplateVersion)}" = @TemplateVersion;

                DELETE FROM "{nameof(InventoryContext.InventoryTemplateAttributes)}"
                WHERE "{nameof(InventoryTemplateAttribute.InventoryTemplateName)}" = @TemplateName AND "{nameof(InventoryTemplateAttribute.InventoryTemplateVersion)}" = @TemplateVersion;
                """;
            var tempAttrQuery = $"""
                INSERT INTO "{nameof(InventoryContext.InventoryTemplateAttributes)}"
                ("{nameof(InventoryTemplateAttribute.InventoryTemplateName)}", "{nameof(InventoryTemplateAttribute.InventoryTemplateVersion)}", "{nameof(InventoryTemplateAttribute.InventoryTemplateAttributeName)}", "{nameof(InventoryTemplateAttribute.InventoryTemplateAttributeAction)}", "{nameof(InventoryTemplateAttribute.InventoryTemplateAttributeValue)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrAction, @AttrValue);
                """;

            var tempAttrPermQuery = $"""
                INSERT INTO "{nameof(InventoryContext.InventoryTemplateAttributeReads)}"
                ("{nameof(InventoryTemplateAttributeRead.InventoryTemplateName)}", "{nameof(InventoryTemplateAttributeRead.InventoryTemplateVersion)}", "{nameof(InventoryTemplateAttributeRead.InventoryTemplateAttributeName)}", "{nameof(InventoryTemplateAttributeRead.Permission)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrPerm);
                """;

            var entAttrQuery = $"""
                INSERT INTO "{nameof(InventoryContext.InventoryTemplateEntityAttributes)}"
                ("{nameof(InventoryTemplateEntityAttribute.InventoryTemplateName)}", "{nameof(InventoryTemplateEntityAttribute.InventoryTemplateVersion)}", "{nameof(InventoryTemplateEntityAttribute.InventoryTemplateEntityAttributeName)}", "{nameof(InventoryTemplateEntityAttribute.InventoryTemplateEntityAttributeAction)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrAction);
                """;

            var entAttrPermQuery = $"""
                INSERT INTO "{nameof(InventoryContext.InventoryTemplateEntityAttributesPermissions)}"
                ("{nameof(InventoryTemplateEntityAttributePermission.InventoryTemplateName)}", "{nameof(InventoryTemplateEntityAttributePermission.InventoryTemplateVersion)}", "{nameof(InventoryTemplateEntityAttributePermission.InventoryTemplateEntityAttributeName)}", "{nameof(InventoryTemplateEntityAttributePermission.Permission)}", "{nameof(InventoryTemplateEntityAttributePermission.Writeable)}")
                VALUES (@TemplateName, @TemplateVersion, @AttrName, @AttrPerm, @Writeable);
                """;

            if (_dbConnection.State != ConnectionState.Open)
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
    }
}
