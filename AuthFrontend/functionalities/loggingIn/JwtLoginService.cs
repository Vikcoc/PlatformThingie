using AuthFrontend.functionalities.loggingIn.JwtStuff;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace AuthFrontend.functionalities.loggingIn
{
    internal sealed class JwtLoginService : ILogInService
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public JwtLoginService(JwtSecurityTokenHandler tokenHandler)
        {
            _tokenHandler = tokenHandler;
        }

        public string MakeAccessToken(UserInfoDto userInfo)
        {
            return userInfo.Email + " " + userInfo.UserName;
        }

        public async Task<UserInfoDto?> ValidateToken(string token, IJwtKeySetGetter keySetGetter, IJwtValidationParamsGetter parameterFiller)
        {
            var keySet = await keySetGetter.GetKeySet();
            if(keySet == null)
                return null;

            var parameters = parameterFiller.FillParameters(keySet);

            JwtSecurityToken parsedToken;
            try
            {
                _tokenHandler.ValidateToken(token, parameters, out var abstractToken);
                parsedToken = (JwtSecurityToken)abstractToken;
            }
            catch (Exception) { return null;}

            var props = parsedToken.Claims.ToDictionary(x => x.Type, x => x.Value);

            return new UserInfoDto
            {
                Email = props["email"],
                UserName = props["name"],
            };
        }
    }
}
