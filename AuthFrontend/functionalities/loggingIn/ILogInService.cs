namespace AuthFrontend.functionalities.loggingIn
{
    public interface ILogInService
    {
        UserInfoDto? ValidateToken(string token);
        string MakeAccessToken(UserInfoDto userInfo);
    }
}
