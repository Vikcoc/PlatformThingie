﻿using AuthFrontend.entities;
using AuthFrontend.seeds;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace AuthFrontend.functionalities.loggingIn.Repositories
{
    public class PAuthRepo([FromKeyedServices("Auth")] IDbConnection dbConnection)
    {
        private readonly IDbConnection _dbConnection = dbConnection;

        public async Task<Guid> CreateUser(UserInfoDto user)
        {
            //how to get the id if guid would be autogenerated
            //insert into "AuthUsers" ("AuthUserId") values(('b95482aa-0e73-46c4-b947-da782d0a3b60')) returning "AuthUserId" as value;
            var query = $"""
                INSERT INTO "{nameof(AuthContext.AuthUsers)}" ("{nameof(AuthUser.AuthUserId)}") values (@UserId);
                INSERT INTO "{nameof(AuthContext.AuthUserClaims)}" ("{nameof(AuthUserClaim.AuthUserId)}", "{nameof(AuthUserClaim.AuthClaimName)}", "{nameof(AuthUserClaim.ClaimValue)}")
                    VALUES (@UserId, '{SeedAuthClaimNames.Username}', @Username),
                           (@UserId, '{SeedAuthClaimNames.Email}', @Email);
                """;

            var userId = Guid.NewGuid();

            var res = await _dbConnection.ExecuteAsync(query, new
            {
                UserId = userId,
                Username = user.UserName,
                Email = user.Email,
            });

            if(res == 0)
                return default;
            return userId;
        }
    }
}
