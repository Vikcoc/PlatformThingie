using Microsoft.EntityFrameworkCore;

namespace UsersDbComponent.entities
{
    [PrimaryKey(nameof(AuthGroupName), nameof(AuthPermissionName))]
    public class AuthGroupPermission
    {
        public required string AuthGroupName { get; set; }
        public required string AuthPermissionName { get; set; }

        public required AuthGroup AuthGroup { get; set; }
        public required AuthPermission AuthPermission { get; set; }
    }
}
