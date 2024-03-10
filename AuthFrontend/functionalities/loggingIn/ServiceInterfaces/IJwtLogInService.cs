using AuthFrontend.functionalities.loggingIn.DTOs;

namespace AuthFrontend.functionalities.loggingIn.ServiceInterfaces
{
    public interface IJwtLogInService
    {
        Task<UserInfoDto?> ValidateToken(string token);
        Task<string?> MakeAccessToken(UserInfoDto userInfo);
    }
}
