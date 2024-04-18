namespace UserInfo.functionalities.user.dtos
{
    public struct GroupWithPermissionsDto
    {
        public string GroupName { get; set; }
        public string[] Permissions { get; set; }
    }
}
