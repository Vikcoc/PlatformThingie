namespace InventoryInfo.functionalities.readingInventory.DTOs
{
    public struct InventoryCreateEntityDto
    {
        public string TemplateName { get; set; }
        public uint TemplateVersion { get; set; }
        public InventoryCreatePropertyDto[] EntityProperties { get; set; }
    }
}
