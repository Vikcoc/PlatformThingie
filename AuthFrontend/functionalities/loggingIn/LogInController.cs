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
            endpoints.MapPost("/login/token", ProcessToken);
        }

        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<ILogInService, GoogleLoginService>();
            services.AddScoped<JwtSecurityTokenHandler>();
            services.AddScoped<HttpClient>();
        }

        public static async Task<IResult> ProcessToken([FromBody] string token, [FromServices] ILogInService service)
        {
            var userInfo = await service.ValidateToken(token);
            if (!userInfo.HasValue)
                return TypedResults.BadRequest("Bad token");

            var resultToken = service.MakeAccessToken(userInfo.Value);

            if (string.IsNullOrWhiteSpace(resultToken))
                return TypedResults.Problem("Cannot make access token");

            return TypedResults.Ok(resultToken);
        }
    }
}
