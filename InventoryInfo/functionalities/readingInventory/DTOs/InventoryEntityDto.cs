namespace InventoryInfo.functionalities.readingInventory.DTOs
{
    public struct InventoryEntityDto
    {
        public Guid InventoryEntityId {  get; set; }
        public InventoryPropertyDto[] EntityProperties { get; set; }
        public InventoryPropertyDto[] TemplateProperties { get; set; }
    }
}
