namespace InventoryTemplateConsumer.dtos
{
    public struct AttrWithPerm
    {
        public string AttrName { get; set; }
        public string AttrValue { get; set; }
        public string AttrAction { get; set; }
        public string[] Permissions { get; set; }
    }
}
