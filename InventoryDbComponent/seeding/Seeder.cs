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

            if (await db.InventoryEntities.AnyAsync())
                return;

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
                InventoryTemplateEntityAttributeAction = "/inventory/actions/someaction",
            };
            var testAttr2 = new InventoryTemplateEntityAttribute
            {
                InventoryTemplate = testTemp,
                InventoryTemplateEntityAttributeName = "Header Name",
                InventoryTemplateName = testTemp.InventoryTemplateName,
                InventoryTemplateVersion = testTemp.InventoryTemplateVersion,
                InventoryTemplateEntityAttributeAction = "/inventory/actions/displayheader",
            };
            db.InventoryTemplateEntityAttributes.Add(testAttr);
            db.InventoryTemplateEntityAttributes.Add(testAttr2);
            var testAttrVal = new InventoryEntityAttributeValue
            { 
                InventoryEntity = testEntity,
                InventoryEntityId = testEntity.InventoryEntityId,
                InventoryTemplateEntityAttribute = testAttr,
                InventoryTemplateEntityAttributeName = testAttr.InventoryTemplateEntityAttributeName,
                InventoryTemplateName = testAttr.InventoryTemplateName,
                InventoryTemplateVersion = testAttr.InventoryTemplateVersion,
                Value = "/favicon.ico"
            };
            var testAttrVal2 = new InventoryEntityAttributeValue
            {
                InventoryEntity = testEntity,
                InventoryEntityId = testEntity.InventoryEntityId,
                InventoryTemplateEntityAttribute = testAttr2,
                InventoryTemplateEntityAttributeName = testAttr2.InventoryTemplateEntityAttributeName,
                InventoryTemplateName = testAttr2.InventoryTemplateName,
                InventoryTemplateVersion = testAttr2.InventoryTemplateVersion,
                Value = "The Name"
            };
            db.InventoryEntitiesAttributeValues.Add(testAttrVal);
            db.InventoryEntitiesAttributeValues.Add(testAttrVal2);
            var testAttrPer = new InventoryTemplateEntityAttributePermission
            {
                InventoryTemplateEntityAttribute = testAttr,
                InventoryTemplateEntityAttributeName = testAttr.InventoryTemplateEntityAttributeName,
                InventoryTemplateName = testAttr.InventoryTemplateName,
                InventoryTemplateVersion = testAttr.InventoryTemplateVersion,
                Permission = "string",
                Writeable = true
            };
            var testAttrPer2 = new InventoryTemplateEntityAttributePermission
            {
                InventoryTemplateEntityAttribute = testAttr2,
                InventoryTemplateEntityAttributeName = testAttr2.InventoryTemplateEntityAttributeName,
                InventoryTemplateName = testAttr2.InventoryTemplateName,
                InventoryTemplateVersion = testAttr2.InventoryTemplateVersion,
                Permission = "string",
                Writeable = true
            };
            db.InventoryTemplateEntityAttributesPermissions.Add(testAttrPer);
            db.InventoryTemplateEntityAttributesPermissions.Add(testAttrPer2);
            var testTpAttr = new InventoryTemplateAttribute
            {
                InventoryTemplate = testTemp,
                InventoryTemplateAttributeAction = "/inventory/actions/someaction2",
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
