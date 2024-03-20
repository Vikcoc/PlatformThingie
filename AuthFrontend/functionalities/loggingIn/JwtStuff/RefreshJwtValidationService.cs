using AuthFrontend.functionalities.loggingIn.DTOs;
using AuthFrontend.functionalities.loggingIn.Repositories;
using AuthFrontend.functionalities.loggingIn.ServiceInterfaces;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    public class RefreshJwtValidationService(IAuthRepo pAuthRepo
            , JwtSecurityTokenHandler tokenHandler
            , [FromKeyedServices("Refresh")] IJwtValidationParamsGetter paramsGetter)
        : JwtLoginService(tokenHandler, paramsGetter)
    {
        private readonly IAuthRepo _pAuthRepo = pAuthRepo;

        public async override Task<UserInfoDto?> ValidateToken(string token)
        {
            var parsedToken = await GetValidToken(token);
            if (parsedToken == null)
                return null;

            var jti = Guid.Parse((string)parsedToken.Header["jti"]);

            var (tokenHash, tokenSalt) = await _pAuthRepo.GetTokenHashAndSalt(jti);

            if (string.IsNullOrWhiteSpace(tokenHash))
                return null;

            if (Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token + tokenSalt))) != tokenHash)
                return null;

            var props = parsedToken.Claims.ToDictionary(x => x.Type, x => x.Value);

            // Unnecessary because only refresh is saved
            // But will stay
            if (props["Purpose"] != "Refresh")
                return null;

            //todo: maybe invalidate the used refresh token
            return new UserInfoDto
            {
                Email = props["email"],
                UserName = props["username"],
            };
        }
    }
}
