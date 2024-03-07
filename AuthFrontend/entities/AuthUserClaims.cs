using System.ComponentModel.DataAnnotations.Schema;

namespace AuthFrontend.entities
{
    internal class AuthUserClaims
    {
        public int AuthUserClaimId { get; set; }
        public Guid AuthUserId { get; set; }
        public string AuthClaimName { get; set; } = string.Empty;

        public string ClaimValue { get; set; } = string.Empty;
    }
}
