using AuthFrontend.entities;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace UserInfo.functionalities.user.Repositories
{
    internal class PProfileRepo([FromKeyedServices("User")] IDbConnection dbConnection)
    {
        private readonly IDbConnection _dbConnection = dbConnection;

        public async Task<IList<(string AuthClaimName, string AuthClaimValue)>> GetUserClaimsByUserId(Guid userId)
        {
            var query = $"""
                SELECT "{nameof(AuthUserClaim.AuthClaimName)}" as "AuthClaimName", "{nameof(AuthUserClaim.AuthClaimValue)}" as "AuthClaimValue"
                FROM "{nameof(AuthContext.AuthUserClaims)}"
                WHERE "{nameof(AuthUserClaim.AuthUserId)}" = @UserId;
                """;

            var res = await _dbConnection.QueryAsync<(string AuthClaimName, string AuthClaimValue)>(query, new
            {
                UserId = userId
            });

            return res.ToList();
        }
    }
}
