using System.ComponentModel.DataAnnotations;

namespace UsersDbComponent.entities
{
    public class AuthGroup
    {
        [Key]
        public required string AuthGroupName { get; set; }

        public List<AuthGroupPermission> GroupPermissions { get; set; } = [];
        public List<AuthUserGroup> UserGroups { get; set; } = [];
    }
}
