namespace InvTemplateInfo.functionalities.permission.dtos
{
    public struct TemplateWithAttributesDto
    {
        public string TemplateName { get; set; }
        public int TemplateVersion { get; set; }
        public string[] Attributes { get; set; }
        public string[] EntityAttributes { get; set; }
    }
}
