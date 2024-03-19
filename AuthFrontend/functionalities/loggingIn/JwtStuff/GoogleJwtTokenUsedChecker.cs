using AuthFrontend.functionalities.loggingIn.DTOs;
using AuthFrontend.functionalities.loggingIn.Repositories;
using AuthFrontend.functionalities.loggingIn.ServiceInterfaces;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    /// <summary>
    /// Used to add the used Google tokens to the db so that they cannot be reused
    /// Not the most straight forward way to do this (change the jti to string), but not too complicated
    /// </summary>
    /// <param name="authRepo"></param>
    internal class GoogleJwtTokenUsedChecker(IAuthRepo authRepo) : IJwtTokenUsedChecker
    {
        private readonly IAuthRepo _authRepo = authRepo;

        public async Task<bool> TokenUsed(string token)
        {
            var parsedToken = new JsonWebToken(token);

            return await _authRepo.CheckHashExists(parsedToken.Id);
        }

        public async Task<bool> UseToken(string token, UserInfoDto user)
        {
            var parsedToken = new JsonWebToken(token);

            //may optimize with repo method to insert by email
            var userId = await _authRepo.GetUserByEmail(user.Email);
            if(!userId.HasValue)
                return false;

            return await _authRepo.AddRefreshToken(new DTOs.RefreshTokenDto
            {
                AuthUserId = userId.Value,
                Expire = new DateTimeOffset(parsedToken.ValidTo).ToUnixTimeMilliseconds(),
                HashedToken = parsedToken.Id,
                JTI = Guid.NewGuid(),
                Salt = string.Empty
            });
        }
    }
}
