using AuthFrontend.functionalities.loggingIn.DTOs;

namespace AuthFrontend.functionalities.loggingIn.Repositories
{
    public interface IAuthRepo
    {
        Task<bool> AddRefreshToken(RefreshTokenDto token);
        Task<Guid?> CreateUser(UserInfoDto user);
        Task<(string tokenHash, string tokenSalt)> GetTokenHashAndSalt(Guid jti);
        Task<Guid?> GetUserByEmail(string email);
        Task<IGrouping<Guid, string>?> GetUserByEmailWithPermissions(string email);
        //Use for google jti because not guid, or other jti, so that we can ensure single use
        public Task<bool> CheckHashExists(string hash);
        public Task<bool> RemoveToken(Guid jti);
    }
}