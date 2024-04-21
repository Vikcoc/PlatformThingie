namespace InvTemplateInfo.functionalities.invtemplate.dtos
{
    public struct TemplateWithAttrAndPerm
    {
        public string TemplateName { get; set; }
        public int TemplateVersion { get; set; }
        public bool Released { get; set; }
        public bool Latest { get; set; }
        public AttrWithPerm[] TemplateAttributes { get; set; }
        public EntAttrWithPerm[] EntityAttributes { get; set; }
    }
}
