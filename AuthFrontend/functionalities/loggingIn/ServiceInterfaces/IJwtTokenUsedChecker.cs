using AuthFrontend.functionalities.loggingIn.DTOs;

namespace AuthFrontend.functionalities.loggingIn.ServiceInterfaces
{
    public interface IJwtTokenUsedChecker
    {
        public Task<bool> TokenUsed(string token);
        public Task<bool> UseToken(string token, UserInfoDto user);
    }
}
