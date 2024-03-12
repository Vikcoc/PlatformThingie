using AuthFrontend.functionalities.loggingIn.DTOs;
using AuthFrontend.functionalities.loggingIn.Repositories;
using AuthFrontend.functionalities.loggingIn.ServiceInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    internal class RefreshJwtValidationService(IConfiguration configuration, PAuthRepo pAuthRepo, JwtSecurityTokenHandler tokenHandler) : IJwtLogValidatorService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly PAuthRepo _pAuthRepo = pAuthRepo;
        private readonly JwtSecurityTokenHandler _tokenHandler = tokenHandler;

        public async Task<UserInfoDto?> ValidateToken(string token)
        {
            var key = _configuration.GetSection("SigningCredentials:Refresh:JWK").Get<JsonWebKey>();
            var keySet = new JsonWebKeySet();
            keySet.Keys.Add(key);
            var parameters = new TokenValidationParameters()
            {
                ValidIssuer = _configuration["SigningCredentials:Refresh:Issuer"],
                ValidAudience = _configuration["SigningCredentials:Refresh:Audience"],
                IssuerSigningKeys = keySet.Keys,
            };

            JwtSecurityToken parsedToken;
            try
            {
                _tokenHandler.ValidateToken(token, parameters, out var abstractToken);
                parsedToken = (JwtSecurityToken)abstractToken;
            }
            catch (Exception) { return null; }

            var jti = Guid.Parse((string)parsedToken.Header["jti"]);

            var (tokenHash, tokenSalt) = await _pAuthRepo.GetTokenHashAndSalt(jti);

            if(string.IsNullOrWhiteSpace(tokenHash)) 
                return null;

            if(Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token + tokenSalt))) != tokenHash)
                return null;

            var props = parsedToken.Claims.ToDictionary(x => x.Type, x => x.Value);

            // Unnecessary because only refresh is saved
            // But will stay
            if(props["Purpose"] != "Refresh")
                return null;

            return new UserInfoDto
            {
                Email = props["email"],
                UserName = props["username"],
            };
        }
    }
}
