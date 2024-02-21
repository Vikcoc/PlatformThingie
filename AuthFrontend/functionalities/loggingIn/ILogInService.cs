namespace AuthFrontend.functionalities.loggingIn
{
    public interface ILogInService
    {
        Task<UserInfoDto?> ValidateToken(string token);
        string MakeAccessToken(UserInfoDto userInfo);
    }
}
