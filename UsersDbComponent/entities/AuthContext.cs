using Microsoft.EntityFrameworkCore;

namespace AuthFrontend.entities
{
    public class AuthContext : DbContext
    {
        public AuthContext():base(new DbContextOptionsBuilder().UseNpgsql().Options) { }

        public AuthContext(DbContextOptions options): base(options) { }
        

        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<AuthUserClaim> AuthUserClaims { get; set; }
        public DbSet<AuthClaim> AuthClaims { get; set; }
        public DbSet<AuthUserRefreshToken> AuthUserRefreshTokens { get; set; }
    }
}
