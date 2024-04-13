using UsersDbComponent.entities;

namespace AuthFrontend.entities
{
    public class AuthUser
    {
        public Guid AuthUserId { get; set; } = Guid.NewGuid();
        public IList<AuthUserRefreshToken> RefreshTokens { get; set; } = [];
        public IList<AuthUserClaim> AuthUserClaims { get; set; } = [];
        public List<AuthUserGroup> UserGroups { get; set; } = [];
    }
}
