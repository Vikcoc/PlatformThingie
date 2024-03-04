using AuthFrontend.functionalities.loggingIn.JwtStuff;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;
using System.IdentityModel.Tokens.Jwt;


namespace AuthFrontend.functionalities.loggingIn
{
    internal static class LogInController
    {
        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/login/google", ProcessGoogleToken);
            endpoints.MapGet("/login/connection", TryDbConn);
        }

        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<ILogInService, JwtLoginService>();
            services.AddScoped<JwtSecurityTokenHandler>();
            services.AddScoped<HttpClient>();
            services.AddKeyedScoped<IJwtKeySetGetter, GoogleJwtKeySetGetter>("Google");
            services.AddKeyedScoped<IJwtValidationParamsGetter, GoogleJwtValidationParamsGetter>("Google");
            services.AddKeyedScoped<IDbConnection>("Auth", (db, key) => {
                var theConnection = db.GetRequiredService<IConfiguration>()["ConnectionStrings:Auth"];
                return new NpgsqlConnection(theConnection);
            });
        }

        public static async Task<IResult> ProcessGoogleToken([FromBody] string token, [FromServices] ILogInService service
            , [FromKeyedServices("Google")] IJwtKeySetGetter keysetGetter
            , [FromKeyedServices("Google")] IJwtValidationParamsGetter validationParamsGetter)
         => await ProcessGenericToken(token, service, keysetGetter, validationParamsGetter);

        private static async Task<IResult> ProcessGenericToken(string token, ILogInService service, IJwtKeySetGetter keysetGetter, IJwtValidationParamsGetter validationParamsGetter)
        {
            var userInfo = await service.ValidateToken(token, keysetGetter, validationParamsGetter);
            if (!userInfo.HasValue)
                return TypedResults.BadRequest("Bad token");

            var resultToken = service.MakeAccessToken(userInfo.Value);

            if (string.IsNullOrWhiteSpace(resultToken))
                return TypedResults.Problem("Cannot make access token");

            return TypedResults.Ok(resultToken);
        }

        private static async Task<IResult> TryDbConn([FromKeyedServices("Auth")] IDbConnection dbConnection)
        {
            dbConnection.Open();

            var res = await dbConnection.QueryAsync("Select 1 as Value");

            dbConnection.Close();
            return TypedResults.Ok();
        }
    }
}
