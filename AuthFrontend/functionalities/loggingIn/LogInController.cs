using AuthFrontend.functionalities.loggingIn.JwtStuff;
using AuthFrontend.functionalities.loggingIn.Repositories;
using AuthFrontend.functionalities.loggingIn.ServiceInterfaces;
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
        // Because not enough time to properly immerse in
        // https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics
        // This should not be taken as good practice
        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/login/google", ProcessGoogleToken);
            endpoints.MapPost("/login/refresh", ProcessRefreshToken);
            endpoints.MapGet("/login/connection", TryDbConn);
        }

        public static void AddServices(IServiceCollection services)
        {
            services.AddKeyedScoped<IJwtLogValidatorService, GoogleJwtLoginService>("Google");
            services.AddKeyedScoped<IJwtLogValidatorService, RefreshJwtValidationService>("Refresh");
            services.AddScoped<JwtSecurityTokenHandler>();
            services.AddScoped<HttpClient>();
            services.AddKeyedScoped<IJwtValidationParamsGetter, GoogleJwtValidationParamsGetter>("Google");
            services.AddKeyedScoped<IJwtValidationParamsGetter, RefreshJwtValidationParamsGetter>("Refresh");
            services.AddKeyedScoped<IDbConnection>("Auth", (db, key) =>
            {
                var theConnection = db.GetRequiredService<IConfiguration>()["ConnectionStrings:Auth"];
                return new NpgsqlConnection(theConnection);
            });
            services.AddScoped<PAuthRepo>();
            services.AddScoped<AuthTokenProvider>();
            services.AddSingleton<Random>();
        }

        public static async Task<IResult> ProcessGoogleToken([FromBody] string token, [FromKeyedServices("Google")] IJwtLogValidatorService service, [FromServices] AuthTokenProvider tokenProvider)
            => await ProcessGenericToken(token, service, tokenProvider);

        public static async Task<IResult> ProcessRefreshToken([FromBody] string token, [FromKeyedServices("Refresh")] IJwtLogValidatorService service, [FromServices] AuthTokenProvider tokenProvider)
            => await ProcessGenericToken(token, service, tokenProvider);


        private static async Task<IResult> ProcessGenericToken(string token, IJwtLogValidatorService service, AuthTokenProvider tokenProvider)
        {
            var userInfo = await service.ValidateToken(token);
            if (!userInfo.HasValue)
                return TypedResults.BadRequest("Bad token");

            var resultToken = await tokenProvider.MakeAccessToken(userInfo.Value);

            if (!resultToken.HasValue)
                return TypedResults.Problem("Cannot make access token");

            return TypedResults.Ok(resultToken);
        }

        private static async Task<IResult> TryDbConn([FromKeyedServices("Auth")] IDbConnection dbConnection)
        {
            dbConnection.Open();

            var res = await dbConnection.QueryAsync("Select 1 as Value");

            dbConnection.Close();
            return TypedResults.Ok(res);
        }
    }
}
