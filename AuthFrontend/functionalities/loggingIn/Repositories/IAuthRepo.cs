using AuthFrontend.functionalities.loggingIn.DTOs;

namespace AuthFrontend.functionalities.loggingIn.Repositories
{
    public interface IAuthRepo
    {
        Task<bool> AddRefreshToken(RefreshTokenDto token);
        Task<Guid?> CreateUser(UserInfoDto user);
        Task<(string tokenHash, string tokenSalt)> GetTokenHashAndSalt(Guid jti);
        Task<Guid?> GetUserByEmail(string email);
    }
}