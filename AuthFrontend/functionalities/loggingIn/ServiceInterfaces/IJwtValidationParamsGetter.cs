using Microsoft.IdentityModel.Tokens;

namespace AuthFrontend.functionalities.loggingIn.ServiceInterfaces
{
    public interface IJwtValidationParamsGetter
    {
        TokenValidationParameters FillParameters(JsonWebKeySet keySet);
    }
}
