﻿using AuthFrontend.entities;
using AuthFrontend.functionalities.loggingIn.DTOs;
using AuthFrontend.seeds;
using Dapper;
using Dependencies;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Security.Claims;
using UsersDbComponent.entities;

namespace AuthFrontend.functionalities.loggingIn.Repositories
{
    public class PAuthRepo([FromKeyedServices("Auth")] IDbConnection dbConnection) : IAuthRepo
    {
        private readonly IDbConnection _dbConnection = dbConnection;

        public async Task<Guid?> CreateUser(UserInfoDto user)
        {
            //how to get the id if guid would be autogenerated
            //insert into "AuthUsers" ("AuthUserId") values(('b95482aa-0e73-46c4-b947-da782d0a3b60')) returning "AuthUserId" as value;
            var query = $"""
                INSERT INTO "{nameof(AuthContext.AuthUsers)}" ("{nameof(AuthUser.AuthUserId)}") values (@UserId);
                INSERT INTO "{nameof(AuthContext.AuthUserClaims)}" ("{nameof(AuthUserClaim.AuthUserClaimId)}", "{nameof(AuthUserClaim.AuthUserId)}", "{nameof(AuthUserClaim.AuthClaimName)}", "{nameof(AuthUserClaim.AuthClaimValue)}")
                    VALUES (@ClaimId, @UserId, '{SeedAuthClaimNames.Email}', @Email);
                """;

            var userId = Guid.NewGuid();

            var res = await _dbConnection.ExecuteAsync(query, new
            {
                ClaimId = Guid.NewGuid(),
                UserId = userId,
                user.Email,
            });

            if (res == 0)
                return null;
            return userId;
        }

        public async Task<Guid?> GetUserByEmail(string email)
        {
            var query = $"""
                SELECT usr."{nameof(AuthUser.AuthUserId)}" as Value
                FROM "{nameof(AuthContext.AuthUsers)}" usr
                JOIN "{nameof(AuthContext.AuthUserClaims)}" clm
                ON usr."{nameof(AuthUser.AuthUserId)}" = clm."{nameof(AuthUserClaim.AuthUserId)}"
                WHERE clm."{nameof(AuthUserClaim.AuthClaimValue)}" = @Email
                LIMIT 1;
                """;
            //todo make the query prioritise users that do not reference another user

            var res = await _dbConnection.QueryAsync<Guid?>(query, new
            {
                Email = email
            });

            return res.FirstOrDefault();
        }


        public async Task<IGrouping<Guid, string>?> GetUserByEmailWithPermissions(string email)
        {
            var query = $"""
                SELECT usr."{nameof(AuthUser.AuthUserId)}" as User, gp."{nameof(AuthGroupPermission.AuthPermissionName)}" as Permission
                FROM "{nameof(AuthContext.AuthUsers)}" usr
                JOIN "{nameof(AuthContext.AuthUserClaims)}" clm
                ON usr."{nameof(AuthUser.AuthUserId)}" = clm."{nameof(AuthUserClaim.AuthUserId)}"
                LEFT JOIN "{nameof(AuthContext.AuthUserGroups)}" ug
                ON usr."{nameof(AuthUser.AuthUserId)}" = ug."{nameof(AuthUserGroup.AuthUserId)}"
                LEFT JOIN "{nameof(AuthContext.AuthGroups)}" g
                ON ug."{nameof(AuthUserGroup.AuthGroupName)}" = g."{nameof(AuthGroup.AuthGroupName)}"
                LEFT JOIN "{nameof(AuthContext.AuthGroupPermissions)}" gp
                ON g."{nameof(AuthGroup.AuthGroupName)}" = gp."{nameof(AuthGroupPermission.AuthGroupName)}"
                WHERE clm."{nameof(AuthUserClaim.AuthClaimValue)}" = @Email;
                """;
            //todo make the query prioritise users that do not reference another user

            var res = await _dbConnection.QueryAsync<(Guid User, string Permission)>(query, new
            {
                Email = email
            });

            if (!res.Any())
                return null;

            var grp = res.GroupBy(x => x.User).First();

            return new Grouping<Guid, string>
            {
                Key = grp.Key,
                Values = grp.Select(x => x.Permission)
            };
        }

        public async Task<(string tokenHash, string tokenSalt)> GetTokenHashAndSalt(Guid jti)
        {
            var query = $"""
                SELECT "{nameof(AuthUserRefreshToken.HashedToken)}" "Hash", "{nameof(AuthUserRefreshToken.Salt)}" as "Salt"
                FROM "{nameof(AuthContext.AuthUserRefreshTokens)}" usr
                WHERE "{nameof(AuthUserRefreshToken.JTI)}" = @JTI
                LIMIT 1;
                """;

            var res = await _dbConnection.QueryAsync<(string Hash, string Salt)>(query, new
            {
                JTI = jti
            });

            return res.FirstOrDefault();
        }

        public async Task<bool> AddRefreshToken(RefreshTokenDto token)
        {
            var query = $"""
                INSERT INTO "{nameof(AuthContext.AuthUserRefreshTokens)}" 
                ("{nameof(AuthUserRefreshToken.JTI)}", "{nameof(AuthUserRefreshToken.AuthUserId)}", "{nameof(AuthUserRefreshToken.Expire)}", "{nameof(AuthUserRefreshToken.Salt)}", "{nameof(AuthUserRefreshToken.HashedToken)}")
                values (@JTI, @AuthUserId, @Expire, @Salt, @HashedToken);
                """;

            var res = await _dbConnection.ExecuteAsync(query, new
            {
                JTI = token.JTI!,
                AuthUserId = token.AuthUserId!,
                Expire = token.Expire!,
                Salt = token.Salt!,
                HashedToken = token.HashedToken!
            });

            return !(res == 0);
        }

        public async Task<bool> CheckHashExists(string hash)
        {
            var query = $"""
                SELECT "{nameof(AuthUserRefreshToken.HashedToken)}" Value
                FROM "{nameof(AuthContext.AuthUserRefreshTokens)}"
                WHERE "{nameof(AuthUserRefreshToken.HashedToken)}" = @HashedToken
                LIMIT 1;
                """;

            var res = await _dbConnection.QueryAsync(query, new
            {
                HashedToken = hash
            });

            return res.Any();
        }

        public async Task<bool> RemoveToken(Guid jti)
        {
            var query = $"""
                DELETE FROM "{nameof(AuthContext.AuthUserRefreshTokens)}" 
                WHERE "{nameof(AuthUserRefreshToken.JTI)}" = @JTI;
                """;

            var res = await _dbConnection.ExecuteAsync(query, new
            {
                JTI = jti,
            });

            return !(res == 0);
        }
    }
}
