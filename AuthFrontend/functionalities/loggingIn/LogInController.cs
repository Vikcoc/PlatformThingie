using AuthFrontend.functionalities.loggingIn.JwtStuff;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

namespace AuthFrontend.functionalities.loggingIn
{
    internal static class LogInController
    {
        public static void AddRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/login/google", ProcessGoogleToken);
        }

        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<ILogInService, JwtLoginService>();
            services.AddScoped<JwtSecurityTokenHandler>();
            services.AddScoped<HttpClient>();
            services.AddKeyedScoped<IJwtKeySetGetter, GoogleJwtKeySetGetter>("Google");
            services.AddKeyedScoped<IJwtValidationParamsGetter, GoogleJwtValidationParamsGetter>("Google");
        }

        public static async Task<IResult> ProcessGoogleToken([FromBody] string token, [FromServices] ILogInService service, [FromServices] IServiceProvider provider)
        {
            var keysetGetter = provider.GetRequiredKeyedService<IJwtKeySetGetter>("Google");
            var validationParamsGetter = provider.GetRequiredKeyedService<IJwtValidationParamsGetter>("Google");

            var userInfo = await service.ValidateToken(token, keysetGetter, validationParamsGetter);
            if (!userInfo.HasValue)
                return TypedResults.BadRequest("Bad token");

            var resultToken = service.MakeAccessToken(userInfo.Value);

            if (string.IsNullOrWhiteSpace(resultToken))
                return TypedResults.Problem("Cannot make access token");

            return TypedResults.Ok(resultToken);
        }
    }
}
