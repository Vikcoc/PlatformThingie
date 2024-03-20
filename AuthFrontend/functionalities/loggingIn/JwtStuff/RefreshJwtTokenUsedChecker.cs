using AuthFrontend.functionalities.loggingIn.DTOs;
using AuthFrontend.functionalities.loggingIn.Repositories;
using AuthFrontend.functionalities.loggingIn.ServiceInterfaces;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    internal class RefreshJwtTokenUsedChecker(IAuthRepo authRepo) : IJwtTokenUsedChecker
    {
        private readonly IAuthRepo _authRepo = authRepo;

        public async Task<bool> TokenUsed(string token)
        {
            var parsedToken = new JsonWebToken(token);
            var (hash, _) = await _authRepo.GetTokenHashAndSalt(Guid.Parse(parsedToken.Id));
            return hash == null;
        }

        public async Task<bool> UseToken(string token, UserInfoDto user)
        {
            var parsedToken = new JsonWebToken(token);

            return await _authRepo.RemoveToken(Guid.Parse(parsedToken.Id));
        }
    }
}
