using AuthFrontend.entities;
using AuthFrontend.seeds;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using UserInfo.functionalities.user.dtos;
using UsersDbComponent.entities;
using static Dapper.SqlMapper;

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

        public async Task DeleteAndRewriteEditableClaims(Guid userId, List<UserClaimDto> claims)
        {
            var query = $"""
                DELETE FROM "{nameof(AuthContext.AuthUserClaims)}" uc
                USING "{nameof(AuthContext.AuthClaims)}" c
                WHERE uc."{nameof(AuthUserClaim.AuthUserId)}" = @UserId
                AND c."{nameof(AuthClaim.AuthClaimRight)}" = @ClaimRight
                AND uc."{nameof(AuthUserClaim.AuthClaimName)}" = c."{nameof(AuthClaim.AuthClaimName)}";
                """;
            var dbArgs = new DynamicParameters();
            dbArgs.Add("@UserId", userId);
            dbArgs.Add("@ClaimRight", AuthClaimRights.Editable);

            //todo clean this thing use the array, use transactions like on inventory
            #region ListOfParams
            var paramIndex = 1;
            var theData = claims.Select(item => {
                var res = new Dictionary<string, object>
                {
                    {"@param" + (paramIndex + 0), Guid.NewGuid()},
                    {"@param" + (paramIndex + 1), userId},
                    {"@param" + (paramIndex + 2), item.AuthClaimName},
                    {"@param" + (paramIndex + 3), item.AuthClaimValue}
                };
                paramIndex += 4;
                return res;
            }).ToList();
            if (theData.Count > 0)
            {
                var bulkEnding = theData
                    .Select(x => "(" + x.Select(y => y.Key).Aggregate((a, b) => a + "," + b) + ")")
                    .Aggregate((a, b) => a + ",\n" + b);
                
                foreach (var pair in theData.SelectMany(x => (IEnumerable<KeyValuePair<string, object>>)x))
                    dbArgs.Add(pair.Key, pair.Value);

                query = query +
                    $"""

                    INSERT INTO "{nameof(AuthContext.AuthUserClaims)}"
                        ("{nameof(AuthUserClaim.AuthUserClaimId)}"
                        , "{nameof(AuthUserClaim.AuthUserId)}"
                        , "{nameof(AuthUserClaim.AuthClaimName)}"
                        , "{nameof(AuthUserClaim.AuthClaimValue)}")
                        values {bulkEnding}
                    """;
            }
            #endregion

            await _dbConnection.ExecuteAsync(query, dbArgs);
        }

        private struct UserIdWithString
        {
            public Guid UserId {  get; set; }
            public string Name { get; set; }
        }
        public async Task<UserWithEmailAndGroupDto[]> GetUsersWithEmailAndGroups()
        {
            var query = $"""
                SELECT "{nameof(AuthUserClaim.AuthUserId)}" as UserId, "{nameof(AuthUserClaim.AuthClaimValue)}" as Name
                FROM "{nameof(AuthContext.AuthUserClaims)}"
                WHERE "{nameof(AuthUserClaim.AuthClaimName)}" = @Claim;

                SELECT "{nameof(AuthUserGroup.AuthUserId)}" as UserId, "{nameof(AuthUserGroup.AuthGroupName)}" as Name
                FROM "{nameof(AuthContext.AuthUserGroups)}"
                """;

            var res = await _dbConnection.QueryMultipleAsync(query, new
            {
                Claim = SeedAuthClaimNames.Email
            });
            var emails = await res.ReadAsync<UserIdWithString>();
            var groups = await res.ReadAsync<UserIdWithString>();

            var users = emails.GroupBy(x => x.UserId);
            var dtos = users
                .Join(groups.GroupBy(x => x.UserId)
                , a => a.Key
                , b => b.Key
                , (a, b) => new UserWithEmailAndGroupDto
                {
                    Emails = a.Select(x => x.Name).ToArray(),
                    UserId = a.Key,
                    Groups = b.Select(x => x.Name).ToArray()
                }).ToList();

            dtos.AddRange(users.Where(x => !dtos.Any(y => y.UserId == x.Key))
                .Select(x => new UserWithEmailAndGroupDto
                {
                    UserId = x.Key,
                    Emails = x.Select(y => y.Name).ToArray(),
                    Groups = []
                }));

            return [.. dtos];
        }

        public async Task<string[]> GetGroups()
        {
            var query = $"""
                SELECT "{nameof(AuthGroup.AuthGroupName)}" as Value
                FROM "{nameof(AuthContext.AuthGroups)}"
                """;
            var res = await _dbConnection.QueryAsync<string>(query);
            return res.ToArray();
        }

        public async Task DeleteAndRewriteUserGroups(Guid userId, IEnumerable<string> groups)
        {
            var query = $"""
                DELETE FROM "{nameof(AuthContext.AuthUserGroups)}"
                WHERE "{nameof(AuthUserGroup.AuthUserId)}" = @UserId;
                """;
            var query2 = $"""
                INSERT INTO "{nameof(AuthContext.AuthUserGroups)}"
                ("{nameof(AuthUserGroup.AuthUserId)}",
                "{nameof(AuthUserGroup.AuthGroupName)}")
                VALUES (@UserId, @Group)
                """;

            _dbConnection.Open();
            using var transaction = _dbConnection.BeginTransaction();

            var res = await _dbConnection.ExecuteAsync(query, new
            {
                UserId = userId
            });

            var res2 = await _dbConnection.ExecuteAsync(query2, groups.Select(g => new
            {
                UserId = userId,
                Group = g
            }).ToArray());

            transaction.Commit();
        }
    }
}
