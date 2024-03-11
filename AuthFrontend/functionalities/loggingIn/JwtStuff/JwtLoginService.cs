using AuthFrontend.functionalities.loggingIn.DTOs;
using AuthFrontend.functionalities.loggingIn.Repositories;
using AuthFrontend.functionalities.loggingIn.ServiceInterfaces;
using AuthFrontend.seeds;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    public abstract class JwtLoginService(JwtSecurityTokenHandler tokenHandler, IJwtKeySetGetter jwtKeySetGetter, IJwtValidationParamsGetter jwtValidationParamsGetter) : IJwtLogValidatorService
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = tokenHandler;
        private readonly IJwtKeySetGetter _jwtKeySetGetter = jwtKeySetGetter;
        private readonly IJwtValidationParamsGetter _jwtValidationParamsGetter = jwtValidationParamsGetter;

        public abstract Task<UserInfoDto?> ValidateToken(string token);

        protected async Task<JwtSecurityToken?> GetValidToken(string token)
        {
            var keySet = await _jwtKeySetGetter.GetKeySet();
            if (keySet == null)
                return null;

            var parameters = _jwtValidationParamsGetter.FillParameters(keySet);

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
