using System.ComponentModel.DataAnnotations;

namespace AuthFrontend.entities
{
    public class AuthUserRefreshToken
    {
        [Key]
        public Guid JTI { get; set; }
        public Guid AuthUserId { get; set; }
        public string HashedToken { get; set; } = string.Empty;
        public long Expire { get; set; }
        public string Salt { get; set; } = string.Empty;

        public required AuthUser AuthUser { get; set; }
    }
}
