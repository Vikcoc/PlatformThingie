using Microsoft.EntityFrameworkCore;

namespace UsersDbComponent.entities
{
    public class AuthContext : DbContext
    {
        public AuthContext() : base(new DbContextOptionsBuilder().UseNpgsql().Options) { }

        public AuthContext(DbContextOptions options) : base(options) { }


        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<AuthUserClaim> AuthUserClaims { get; set; }
        public DbSet<AuthClaim> AuthClaims { get; set; }
        public DbSet<AuthUserRefreshToken> AuthUserRefreshTokens { get; set; }
        public DbSet<AuthUserGroup> AuthUserGroups { get; set; }
        public DbSet<AuthGroup> AuthGroups { get; set; }
        public DbSet<AuthGroupPermission> AuthGroupPermissions { get; set; }
        public DbSet<AuthPermission> AuthPermissions { get; set; }
    }
}
