using Microsoft.EntityFrameworkCore;

namespace AuthFrontend.entities
{
    internal class AuthContext : DbContext
    {
        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<AuthUserClaim> AuthUserClaims { get; set; }
        public DbSet<AuthClaim> AuthClaims { get; set; }
        public DbSet<AuthUserRefreshToken> AuthUserRefreshTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql();
        }

    }
}
