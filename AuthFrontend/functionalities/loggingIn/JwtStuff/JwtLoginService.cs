using AuthFrontend.functionalities.loggingIn.DTOs;
using AuthFrontend.functionalities.loggingIn.Repositories;
using AuthFrontend.functionalities.loggingIn.ServiceInterfaces;
using System.IdentityModel.Tokens.Jwt;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    public abstract class JwtLoginService : IJwtLogInService
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly IJwtKeySetGetter _jwtKeySetGetter;
        private readonly IJwtValidationParamsGetter _jwtValidationParamsGetter;
        private readonly PAuthRepo _authRepo;

        public JwtLoginService(JwtSecurityTokenHandler tokenHandler, IJwtKeySetGetter jwtKeySetGetter, IJwtValidationParamsGetter jwtValidationParamsGetter, PAuthRepo authRepo)
        {
            _tokenHandler = tokenHandler;
            _jwtKeySetGetter = jwtKeySetGetter;
            _jwtValidationParamsGetter = jwtValidationParamsGetter;
            _authRepo = authRepo;
        }

        public async Task<string?> MakeAccessToken(UserInfoDto userInfo)
        {
            //todo check if no account

            var id = await _authRepo.CreateUser(userInfo);

            if (id == default)
                return default;

            //todo actually make token
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
