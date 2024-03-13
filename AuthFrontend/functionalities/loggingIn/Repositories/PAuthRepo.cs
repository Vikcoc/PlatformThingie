﻿using AuthFrontend.entities;
using AuthFrontend.functionalities.loggingIn.DTOs;
using AuthFrontend.seeds;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

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
                INSERT INTO "{nameof(AuthContext.AuthUserClaims)}" ("{nameof(AuthUserClaim.AuthUserId)}", "{nameof(AuthUserClaim.AuthClaimName)}", "{nameof(AuthUserClaim.AuthClaimValue)}")
                    VALUES (@UserId, '{SeedAuthClaimNames.Username}', @Username),
                           (@UserId, '{SeedAuthClaimNames.Email}', @Email);
                """;

            var userId = Guid.NewGuid();

            var res = await _dbConnection.ExecuteAsync(query, new
            {
                UserId = userId,
                Username = user.UserName,
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

            var res = await _dbConnection.ExecuteAsync(query, new AuthUserRefreshToken
            {
                AuthUser = null!,
                JTI = token.JTI,
                AuthUserId = token.AuthUserId,
                Expire = token.Expire,
                Salt = token.Salt,
                HashedToken = token.HashedToken
            });

            return !(res == 0);
        }
    }
}
