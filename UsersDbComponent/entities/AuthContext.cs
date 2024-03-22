using Microsoft.EntityFrameworkCore;

namespace AuthFrontend.entities
{
    public class AuthContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<AuthUserClaim> AuthUserClaims { get; set; }
        public DbSet<AuthClaim> AuthClaims { get; set; }
        public DbSet<AuthUserRefreshToken> AuthUserRefreshTokens { get; set; }
    }
}
