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
    public abstract class JwtLoginService : IJwtLogInService
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly IJwtKeySetGetter _jwtKeySetGetter;
        private readonly IJwtValidationParamsGetter _jwtValidationParamsGetter;
        private readonly PAuthRepo _authRepo;
        private readonly IConfiguration _configuration;

        public JwtLoginService(JwtSecurityTokenHandler tokenHandler, IJwtKeySetGetter jwtKeySetGetter, IJwtValidationParamsGetter jwtValidationParamsGetter, PAuthRepo authRepo, IConfiguration configuration)
        {
            _tokenHandler = tokenHandler;
            _jwtKeySetGetter = jwtKeySetGetter;
            _jwtValidationParamsGetter = jwtValidationParamsGetter;
            _authRepo = authRepo;
            _configuration = configuration;
        }

        public async Task<string?> MakeAccessToken(UserInfoDto userInfo)
        {
            var userId = await _authRepo.GetUserByEmail(userInfo.Email);
            if(!userId.HasValue)
                userId = await _authRepo.CreateUser(userInfo);

            if (!userId.HasValue)
                return null;

            var rsa = RSA.Create();
            rsa.ImportFromPem(_configuration["SigningCredentials:PrivateRSAKey"]);
            var securitykey = new RsaSecurityKey(rsa)
            {
                KeyId = _configuration["SigningCredentials:JWK:kid"]
            };

            var desc = new SecurityTokenDescriptor()
            {
                Expires = DateTimeOffset.Now.AddMinutes(10).DateTime,
                SigningCredentials = new SigningCredentials(securitykey, _configuration["SigningCredentials:JWK:alg"]),
                Audience = _configuration["SigningCredentials:Audience"],
                Issuer = _configuration["SigningCredentials:Issuer"],
                NotBefore = DateTimeOffset.Now.DateTime,
                IssuedAt = DateTimeOffset.Now.DateTime,
                TokenType = "JWT",
                AdditionalInnerHeaderClaims = new Dictionary<string, object>() { { "jti", Guid.NewGuid().ToString()} },
                Claims = new Dictionary<string, object>() {
                    { "UserId", userId.Value.ToString() },
                    { SeedAuthClaimNames.Email, userInfo.Email },
                    { SeedAuthClaimNames.Username, userInfo.UserName }
                }
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(desc);

            //todo make and save refresh token

            return token.RawData;
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
