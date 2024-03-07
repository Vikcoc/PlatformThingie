namespace AuthFrontend.functionalities.loggingIn
{
    public interface IJwtLogInService
    {
        Task<UserInfoDto?> ValidateToken(string token);
        string MakeAccessToken(UserInfoDto userInfo);
    }
}
