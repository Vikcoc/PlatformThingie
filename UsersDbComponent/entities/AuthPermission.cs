using System.ComponentModel.DataAnnotations;

namespace UsersDbComponent.entities
{
    public class AuthPermission
    {
        [Key]
        public required string AuthPermissionName {  get; set; }

        public List<AuthGroupPermission> GroupPermissions { get; set; } = [];
    }
}
