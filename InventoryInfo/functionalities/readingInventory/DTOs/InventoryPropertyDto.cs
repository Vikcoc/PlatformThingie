namespace InventoryInfo.functionalities.readingInventory.DTOs
{
    public struct InventoryPropertyDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string ScriptName { get; set; }
        public bool Writeable { get; set; }
    }
}
