using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddScoped<ILogInService, PlaceholderLoginService>();
        }

        public static IResult ProcessToken([FromBody] string token, [FromServices] ILogInService service)
        {
            var userInfo = service.ValidateToken(token);
            if (!userInfo.HasValue)
                return TypedResults.BadRequest("Bad token");

            var resultToken = service.MakeAccessToken(userInfo.Value);

            if (string.IsNullOrWhiteSpace(resultToken))
                return TypedResults.Problem("Cannot make access token");

            return TypedResults.Ok(resultToken);
        }
    }
}
