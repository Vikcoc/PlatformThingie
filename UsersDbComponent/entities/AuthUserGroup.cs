using AuthFrontend.entities;
using Microsoft.EntityFrameworkCore;

namespace UsersDbComponent.entities
{
    [PrimaryKey(nameof(AuthUserId), nameof(AuthGroupName))]
    public class AuthUserGroup
    {
        public required Guid AuthUserId { get; set; }
        public required string AuthGroupName { get; set; }

        public required AuthUser AuthUser { get; set; }
        public required AuthGroup AuthGroup { get; set; }
    }
}
