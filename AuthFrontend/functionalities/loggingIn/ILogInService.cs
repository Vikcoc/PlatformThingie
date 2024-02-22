using AuthFrontend.functionalities.loggingIn.JwtStuff;

namespace AuthFrontend.functionalities.loggingIn
{
    public interface ILogInService
    {
        Task<UserInfoDto?> ValidateToken(string token, IJwtKeySetGetter keySetGetter, IJwtValidationParamsGetter parameterFiller);
        string MakeAccessToken(UserInfoDto userInfo);
    }
}
