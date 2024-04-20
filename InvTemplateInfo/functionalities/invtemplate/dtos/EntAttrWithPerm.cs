namespace InvTemplateInfo.functionalities.invtemplate.dtos
{
    public struct EntAttrWithPerm
    {
        public string AttrName { get; set; }
        public string AttrAction { get; set; }
        public PermissionWithWrite[] Permissions { get; set; }
    }
}
