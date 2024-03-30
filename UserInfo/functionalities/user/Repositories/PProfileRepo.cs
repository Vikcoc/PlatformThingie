using AuthFrontend.entities;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using UsersDbComponent.entities;

namespace UserInfo.functionalities.user.Repositories
{
    internal class PProfileRepo([FromKeyedServices("User")] IDbConnection dbConnection)
    {
        private readonly IDbConnection _dbConnection = dbConnection;

        public async Task<List<(string AuthClaimName, string AuthClaimValue, string AuthClaimRight)>> GetUserClaimsByUserId(Guid userId)
        {
            var query = $"""
                SELECT uc."{nameof(AuthUserClaim.AuthClaimName)}" as "AuthClaimName"
                , uc."{nameof(AuthUserClaim.AuthClaimValue)}" as "AuthClaimValue"
                , cl."{nameof(AuthClaim.AuthClaimRight)}" as "AuthClaimRight"
                FROM "{nameof(AuthContext.AuthUserClaims)}" uc
                JOIN "{nameof(AuthContext.AuthClaims)}" cl ON uc."{nameof(AuthUserClaim.AuthClaimName)}" = cl."{nameof(AuthClaim.AuthClaimName)}"
                WHERE uc."{nameof(AuthUserClaim.AuthUserId)}" = @UserId
                AND cl."{nameof(AuthClaim.AuthClaimRight)}" <> @Claim;
                """;

            var res = await _dbConnection.QueryAsync<(string AuthClaimName, string AuthClaimValue, string AuthClaimRight)>(query, new
            {
                UserId = userId,
                Claim = AuthClaimRights.Invisible
            });

            return res.ToList();
        }

        public async Task<List<string>> GetEditableClaims()
        {
            var query = $"""
                SELECT "{nameof(AuthClaim.AuthClaimName)}" as Value
                FROM "{nameof(AuthContext.AuthClaims)}"
                WHERE "{nameof(AuthClaim.AuthClaimRight)}" = @Claim;
                """;

            var res = await _dbConnection.QueryAsync<string>(query, new
            {
                Claim = AuthClaimRights.Editable
            });

            return res.ToList();
        }
    }
}
