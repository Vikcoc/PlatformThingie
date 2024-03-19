using AuthFrontend.functionalities.loggingIn.Repositories;
using AuthFrontend.functionalities.loggingIn.ServiceInterfaces;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    internal class GoogleJwtTokenUsedChecker(IAuthRepo authRepo) : IJwtTokenUsedChecker
    {
        private readonly IAuthRepo _authRepo = authRepo;

        public async Task<bool> TokenUsed(string token)
        {
            var parsedToken = new JsonWebToken(token);

            return await _authRepo.CheckHashExists(parsedToken.Id);
        }

        public async Task<bool> UseToken(string token, Guid userId)
        {
            var parsedToken = new JsonWebToken(token);

            return await _authRepo.AddRefreshToken(new DTOs.RefreshTokenDto
            {
                AuthUserId = userId,
                Expire = new DateTimeOffset(parsedToken.ValidTo).ToUnixTimeMilliseconds(),
                HashedToken = parsedToken.Id,
                JTI = Guid.NewGuid(),
                Salt = string.Empty
            });
        }
    }
}
