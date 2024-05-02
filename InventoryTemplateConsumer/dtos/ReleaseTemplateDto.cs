namespace InventoryTemplateConsumer.dtos
{
    public struct ReleaseTemplateDto
    {
        public string TemplateName { get; set; }
        public int TemplateVersion { get; set; }
        public AttrWithPerm[] TemplateAttributes { get; set; }
        public EntAttrWithPerm[] EntityAttributes { get; set; }
    }
}
