using Microsoft.IdentityModel.Tokens;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    public interface IJwtValidationParamsGetter
    {
        TokenValidationParameters FillParameters(JsonWebKeySet keySet);
    }
}
