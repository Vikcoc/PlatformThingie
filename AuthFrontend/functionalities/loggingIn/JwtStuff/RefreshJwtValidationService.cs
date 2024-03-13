using AuthFrontend.functionalities.loggingIn.DTOs;
using AuthFrontend.functionalities.loggingIn.Repositories;
using AuthFrontend.functionalities.loggingIn.ServiceInterfaces;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    public class RefreshJwtValidationService : JwtLoginService
    {
        private readonly PAuthRepo _pAuthRepo;

        public RefreshJwtValidationService(PAuthRepo pAuthRepo
            , JwtSecurityTokenHandler tokenHandler
            , [FromKeyedServices("Refresh")] IJwtValidationParamsGetter paramsGetter)
            : base(tokenHandler, paramsGetter)
        {
            _pAuthRepo = pAuthRepo;
        }

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

            return new UserInfoDto
            {
                Email = props["email"],
                UserName = props["username"],
            };
        }
    }
}
