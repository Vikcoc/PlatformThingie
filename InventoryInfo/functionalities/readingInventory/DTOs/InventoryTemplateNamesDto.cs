namespace InventoryInfo.functionalities.readingInventory.DTOs
{
    public struct InventoryTemplateNamesDto
    {
        public string TemplateName { get; set; }
        public int TemplateVersion { get; set; }
        public string[] EntityProperties { get; set; }
        public string[] TemplateProperties { get; set; }
    }
}
