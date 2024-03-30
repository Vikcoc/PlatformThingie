using System.ComponentModel.DataAnnotations.Schema;

namespace AuthFrontend.entities
{
    public class AuthUserClaim
    {
        public Guid AuthUserClaimId { get; set; } = Guid.NewGuid();

        public Guid AuthUserId { get; set; }

        [ForeignKey(nameof(AuthClaim))]
        public required string AuthClaimName { get; set; }

        public required string AuthClaimValue { get; set; }

        public required AuthUser AuthUser { get; set; }
        public required AuthClaim AuthClaim { get; set; }
    }
}
