using System.ComponentModel.DataAnnotations.Schema;

namespace AuthFrontend.entities
{
    internal class AuthUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AuthUserId { get; set; }
        public Guid? NewAuthUserId { get; set; }
        public AuthUser? NewAuthUser { get; set; }
        public IList<AuthUserRefreshToken> RefreshTokens { get; set; } = new List<AuthUserRefreshToken>();
        public IList<AuthUserClaim> AuthUserClaims { get; set; } = new List<AuthUserClaim>();
    }
}
