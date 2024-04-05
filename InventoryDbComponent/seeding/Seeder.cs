using InventoryDbComponent.entities;
using Microsoft.EntityFrameworkCore;
using PlatformInterfaces;

namespace InventoryDbComponent.seeding
{
    public class Seeder : IMigrationProvider
    {
        public string Name => "Inventory";

        public async Task Migrate(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseNpgsql(connectionString);
            var db = new InventoryContext(optionsBuilder.Options);

            await db.Database.MigrateAsync();

            var testEntity = new InventoryEntity();
            db.InventoryEntities.Add(testEntity);
            var testTemp = new InventoryTemplate()
            {
                InventoryTemplateName = "test template",
                InventoryTemplateVersion = 0
            };
            db.InventoryTemplates.Add(testTemp);
            var testAttr = new InventoryTemplateEntityAttribute
            {
                InventoryTemplate = testTemp,
                InventoryTemplateEntityAttributeName = "test attribute",
                InventoryTemplateName = testTemp.InventoryTemplateName,
                InventoryTemplateVersion = testTemp.InventoryTemplateVersion,
                InventoryTemplateEntityAttributeAction = "someaction",
            };
            db.InventoryTemplateEntityAttributes.Add(testAttr);
            var testAttrVal = new InventoryEntityAttributeValue
            { 
                InventoryEntity = testEntity,
                InventoryEntityId = testEntity.InventoryEntityId,
                InventoryTemplateEntityAttribute = testAttr,
                InventoryTemplateEntityAttributeName = testAttr.InventoryTemplateEntityAttributeName,
                InventoryTemplateName = testAttr.InventoryTemplateName,
                InventoryTemplateVersion = testAttr.InventoryTemplateVersion,
                Value = "somevalue"
            };
            db.InventoryEntitiesAttributeValues.Add(testAttrVal);
            var testAttrPer = new InventoryTemplateEntityAttributePermission
            {
                InventoryTemplateEntityAttribute = testAttr,
                InventoryTemplateEntityAttributeName = testAttr.InventoryTemplateEntityAttributeName,
                InventoryTemplateName = testAttr.InventoryTemplateName,
                InventoryTemplateVersion = testAttr.InventoryTemplateVersion,
                Permission = "string",
                Writeable = true
            };
            db.InventoryTemplateEntityAttributesPermissions.Add(testAttrPer);
            var testTpAttr = new InventoryTemplateAttribute
            {
                InventoryTemplate = testTemp,
                InventoryTemplateAttributeAction = "someaction2",
                InventoryTemplateName = testTemp.InventoryTemplateName,
                InventoryTemplateVersion = testTemp.InventoryTemplateVersion,
                InventoryTemplateAttributeName = "test template attribute",
                InventoryTemplateAttributeValue = "somevalue2",
            };
            db.InventoryTemplateAttributes.Add(testTpAttr);
            var testTpAttrPer = new InventoryTemplateAttributeRead
            {
                InventoryTemplateAttribute = testTpAttr,
                InventoryTemplateAttributeName = testTpAttr.InventoryTemplateAttributeName,
                InventoryTemplateName = testTpAttr.InventoryTemplateName,
                InventoryTemplateVersion = testTpAttr.InventoryTemplateVersion,
                Permission = "string",
            };
            db.InventoryTemplateAttributeReads.Add(testTpAttrPer);

            await db.SaveChangesAsync();
        }
    }
}
