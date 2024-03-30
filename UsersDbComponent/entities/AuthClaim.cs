using System.ComponentModel.DataAnnotations;
using UsersDbComponent.entities;

namespace AuthFrontend.entities
{
    public class AuthClaim
    {
        [Key]
        public required string AuthClaimName { get; set; } = string.Empty;
        public required AuthClaimRights AuthClaimRight { get; set; }

        public IList<AuthUserClaim> AuthUserClaims { get; set; } = [];
    }
}
