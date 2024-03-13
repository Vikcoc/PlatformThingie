using AuthFrontend.functionalities.loggingIn.DTOs;

namespace AuthFrontend.functionalities.loggingIn.ServiceInterfaces
{
    public interface IJwtLogValidatorService
    {
        Task<UserInfoDto?> ValidateToken(string token);
    }
}
