using AuthFrontend.functionalities.loggingIn.JwtStuff;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace AuthFrontend.functionalities.loggingIn
{
    internal abstract class JwtLoginService : IJwtLogInService
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly IJwtKeySetGetter _jwtKeySetGetter;
        private readonly IJwtValidationParamsGetter _jwtValidationParamsGetter;

        public JwtLoginService(JwtSecurityTokenHandler tokenHandler, IJwtKeySetGetter jwtKeySetGetter, IJwtValidationParamsGetter jwtValidationParamsGetter)
        {
            _tokenHandler = tokenHandler;
            _jwtKeySetGetter = jwtKeySetGetter;
            _jwtValidationParamsGetter = jwtValidationParamsGetter;
        }

        public string MakeAccessToken(UserInfoDto userInfo)
        {
            return userInfo.Email + " " + userInfo.UserName;
        }

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
