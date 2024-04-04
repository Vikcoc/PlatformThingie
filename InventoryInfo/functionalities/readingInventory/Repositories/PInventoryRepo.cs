using Dapper;
using InventoryDbComponent.entities;
using InventoryInfo.functionalities.readingInventory.DTOs;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace InventoryInfo.functionalities.readingInventory.Repositories
{
    internal class PInventoryRepo([FromKeyedServices("Inventory")] IDbConnection dbConnection)
    {
        private readonly IDbConnection _dbConnection = dbConnection;


        private struct GetEntityAttributeDto
        {
            public Guid EntityId {  get; set; }
            public string AttributeName { get; set; }
            public string AttributeValue { get; set; }
            public string Scipt { get; set; }
            public bool Writeable { get; set; }
            public string TemplateName { get; set; }
            public uint TemplateVersion { get; set; }
        }
        private struct GetTemplateAttributeDto
        {
            public string TemplateName { get; set; }
            public uint TemplateVersion { get; set; }
            public string AttributeName { get; set; }
            public string AttributeValue { get; set; }
            public string Scipt { get; set; }
        }
        public async Task<InventoryEntityDto[]> GetEntities(IEnumerable<string> permissions, IEnumerable<string> entityColumns, IEnumerable<string> templateColumns)
        {
            //I was doing it wrong
            //https://stackoverflow.com/questions/8388093/select-from-x-where-id-in-with-dapper-orm#58448315

            // I need all entity params from permissions -> group by entity param
            // will get entity id and template id

            // I need all template params from permissions -> group by template param
            // will get template id

            // still think more efficient is to join groups in memory after grouping parameters by entity
            // , than to have an sql join and have a cross product of duplicates
            // because many to many hard

            var query = $"""
                SELECT attrv."{nameof(InventoryEntityAttributeValue.InventoryEntityId)}" as "EntityId",
                    attrv."{nameof(InventoryEntityAttributeValue.InventoryTemplateEntityAttributeName)}" as "AttributeName",
                    attrv."{nameof(InventoryEntityAttributeValue.Value)}" as "AttributeValue",
                    attr."{nameof(InventoryTemplateEntityAttribute.InventoryTemplateEntityAttributeAction)}" as "Scipt",
                    MAX(attrp."{nameof(InventoryTemplateEntityAttributePermission.Writeable)}") as "Writeable",
                    attr."{nameof(InventoryTemplateEntityAttribute.InventoryTemplateName)}" as "TemplateName",
                    attr."{nameof(InventoryTemplateEntityAttribute.InventoryTemplateVersion)}" as "TemplateVersion"
                FROM "{nameof(InventoryContext.InventoryEntitiesAttributeValues)}" attrv
                JOIN "{nameof(InventoryContext.InventoryTemplateEntityAttributes)}" attr
                ON attrv."{nameof(InventoryEntityAttributeValue.InventoryTemplateEntityAttributeName)}" = attr."{nameof(InventoryTemplateEntityAttribute.InventoryTemplateName)}"
                    AND attrv."{nameof(InventoryEntityAttributeValue.InventoryTemplateName)}" = attr."{nameof(InventoryTemplateEntityAttribute.InventoryTemplateName)}"
                    AND attrv."{nameof(InventoryEntityAttributeValue.InventoryTemplateVersion)}" = attr."{nameof(InventoryTemplateEntityAttribute.InventoryTemplateVersion)}"
                JOIN "{nameof(InventoryContext.InventoryTemplateEntityAttributesPermissions)}" attrp
                ON attr."{nameof(InventoryTemplateEntityAttribute.InventoryTemplateEntityAttributeName)}" = attrp."{nameof(InventoryTemplateEntityAttributePermission.InventoryTemplateEntityAttributeName)}"
                    AND attr."{nameof(InventoryTemplateEntityAttribute.InventoryTemplateName)}" = attrp."{nameof(InventoryTemplateEntityAttributePermission.InventoryTemplateName)}"
                    AND attr."{nameof(InventoryTemplateEntityAttribute.InventoryTemplateVersion)}" = attrp."{nameof(InventoryTemplateEntityAttributePermission.InventoryTemplateVersion)}"
                WHERE attr."{nameof(InventoryTemplateEntityAttribute.InventoryTemplateEntityAttributeName)}" = ANY(@EntityColumns)
                    AND attrp."{nameof(InventoryTemplateEntityAttributePermission.Permission)}" = ANY(@Permissions)
                GROUP BY attrv."{nameof(InventoryEntityAttributeValue.InventoryEntityId)}"
                    , attrv."{nameof(InventoryEntityAttributeValue.InventoryTemplateEntityAttributeName)}"
                    , attrv."{nameof(InventoryEntityAttributeValue.Value)}"
                    , attr."{nameof(InventoryTemplateEntityAttribute.InventoryTemplateEntityAttributeAction)}"
                    , attr."{nameof(InventoryTemplateEntityAttribute.InventoryTemplateName)}"
                    , attr."{nameof(InventoryTemplateEntityAttribute.InventoryTemplateVersion)}";

                SELECT attr."{nameof(InventoryTemplateAttribute.InventoryTemplateName)}" as "TemplateName",
                    attr."{nameof(InventoryTemplateAttribute.InventoryTemplateVersion)}" as "TemplateVersion",
                    attr."{nameof(InventoryTemplateAttribute.InventoryTemplateAttributeName)}" as "AttributeName",
                    attrv."{nameof(InventoryTemplateAttribute.InventoryTemplateAttributeValue)}" as "AttributeValue",
                    attr."{nameof(InventoryTemplateAttribute.InventoryTemplateAttributeAction)}" as "Scipt"
                FROM "{nameof(InventoryContext.InventoryTemplateAttributes)}" attr
                JOIN "{nameof(InventoryContext.InventoryTemplateEntityAttributesPermissions)}" attrp
                ON attr."{nameof(InventoryTemplateAttribute.InventoryTemplateName)}" = attrp."{nameof(InventoryTemplateAttributeRead.InventoryTemplateName)}"
                    AND attr."{nameof(InventoryTemplateAttribute.InventoryTemplateVersion)}" = attrp."{nameof(InventoryTemplateAttributeRead.InventoryTemplateVersion)}"
                    AND attr."{nameof(InventoryTemplateAttribute.InventoryTemplateAttributeName)}" = attrp."{nameof(InventoryTemplateAttributeRead.InventoryTemplateAttributeName)}"
                WHERE attr."{nameof(InventoryTemplateAttribute.InventoryTemplateAttributeName)}" = ANY(@TemplateColumns)
                    AND attrp."{nameof(InventoryTemplateAttributeRead.Permission)}" = ANY(@Permissions)
                GROUP BY attr."{nameof(InventoryTemplateAttribute.InventoryTemplateName)}"
                    , attr."{nameof(InventoryTemplateAttribute.InventoryTemplateVersion)}"
                    , attr."{nameof(InventoryTemplateAttribute.InventoryTemplateAttributeName)}"
                    , attr."{nameof(InventoryTemplateAttribute.InventoryTemplateAttributeValue)}"
                    , attr."{nameof(InventoryTemplateAttribute.InventoryTemplateAttributeAction)}";
                """;
            //todo make the query prioritise users that do not reference another user

            var res = await _dbConnection.QueryMultipleAsync(query,new
            {
                Permissions = permissions,
                EntityColumns = entityColumns,
                TemplateColumns = templateColumns
            });
            var entityProps = await res.ReadAsync<GetEntityAttributeDto>();
            var templateProps = await res.ReadAsync<GetTemplateAttributeDto>();

            var dtos = entityProps.GroupBy(x => (x.EntityId, x.TemplateName, x.TemplateVersion))
                .Join(templateProps.GroupBy(x => (x.TemplateName, x.TemplateVersion))
                , a => (a.Key.TemplateName, a.Key.TemplateVersion)
                , b => b.Key
                , (a, b) => new InventoryEntityDto
                {
                    InventoryEntityId = a.Key.EntityId,
                    EntityProperties = a.Select(x => new InventoryPropertyDto
                    {
                        Name = x.AttributeName,
                        ScriptName = x.Scipt,
                        Value = x.AttributeValue,
                        Writeable = x.Writeable
                    }).ToArray(),
                    TemplateProperties = b.Select(x => new InventoryPropertyDto
                    {
                        Name = x.AttributeName,
                        ScriptName = x.Scipt,
                        Value = x.AttributeValue,
                        Writeable = false
                    }).ToArray(),
                }).ToArray();

            return dtos;
        }
    }
}
