using Dapper;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using UsersDbComponent.entities;

namespace UserPermissionConsumer
{
    public class PUserPermissionRepo([FromKeyedServices("UserPermission")]IDbConnection dbConnection)
    {
        private readonly IDbConnection _dbConnection = dbConnection;

        public async Task<string[]> OnlyMissing(string[] permissions)
        {
            var query = $"""
                SELECT Value FROM UNNEST(ARRAY[@Permissions]) as Value

                EXCEPT

                SELECT "{nameof(AuthPermission.AuthPermissionName)}" as Value
                FROM "{nameof(AuthContext.AuthPermissions)}";
                """;

            var res = await _dbConnection.QueryAsync<string>(query, new
            {
                Permissions = permissions
            });

            return res.ToArray();
        }

        public async Task AddPermissions(string[] permissions)
        {
            var query = $"""
                INSERT INTO "{nameof(AuthContext.AuthPermissions)}"
                    ("{nameof(AuthPermission.AuthPermissionName)}")
                    values (@Permission);
                """;

            await _dbConnection.ExecuteAsync(query, permissions.Select(x => new {Permission = x}));
        }
    }
}
