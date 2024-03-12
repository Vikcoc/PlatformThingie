using Microsoft.IdentityModel.Tokens;

namespace AuthFrontend.functionalities.loggingIn.ServiceInterfaces
{
    public interface IJwtValidationParamsGetter
    {
        Task<TokenValidationParameters?> FillParameters();
    }
}
