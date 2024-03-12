using AuthFrontend.functionalities.loggingIn.DTOs;
using AuthFrontend.functionalities.loggingIn.ServiceInterfaces;
using System.IdentityModel.Tokens.Jwt;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    public abstract class JwtLoginService(JwtSecurityTokenHandler tokenHandler, IJwtValidationParamsGetter jwtValidationParamsGetter) : IJwtLogValidatorService
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = tokenHandler;
        private readonly IJwtValidationParamsGetter _jwtValidationParamsGetter = jwtValidationParamsGetter;

        public abstract Task<UserInfoDto?> ValidateToken(string token);

        protected async Task<JwtSecurityToken?> GetValidToken(string token)
        {
            var parameters = await _jwtValidationParamsGetter.FillParameters();

            JwtSecurityToken parsedToken;
            try
            {
                _tokenHandler.ValidateToken(token, parameters, out var abstractToken);
                parsedToken = (JwtSecurityToken)abstractToken;
            }
            catch (Exception) { return null; }

            return parsedToken;
        }
    }
}
