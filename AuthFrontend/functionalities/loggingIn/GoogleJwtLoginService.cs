using AuthFrontend.functionalities.loggingIn.JwtStuff;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

namespace AuthFrontend.functionalities.loggingIn
{
    internal class GoogleJwtLoginService : JwtLoginService
    {
        public GoogleJwtLoginService(JwtSecurityTokenHandler tokenHandler
            , [FromKeyedServices("Google")] IJwtKeySetGetter jwtKeySetGetter
            , [FromKeyedServices("Google")] IJwtValidationParamsGetter jwtValidationParamsGetter) 
            : base(tokenHandler, jwtKeySetGetter, jwtValidationParamsGetter)
        {
        }

        public override async Task<UserInfoDto?> ValidateToken(string token)
        {
            var validToken = await GetValidToken(token);

            if (validToken == null)
                return null;

            var props = validToken.Claims.ToDictionary(x => x.Type, x => x.Value);

            return new UserInfoDto
            {
                Email = props["email"],
                UserName = props["name"],
            };
        }

    }
}
