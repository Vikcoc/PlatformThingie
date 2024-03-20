using System.ComponentModel.DataAnnotations;

namespace AuthFrontend.entities
{
    public class AuthClaim
    {
        [Key]
        public string AuthClaimName { get; set; } = string.Empty;

        public IList<AuthUserClaim> AuthUserClaims { get; set; } = [];
    }
}
