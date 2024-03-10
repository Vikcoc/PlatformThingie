using AuthFrontend.functionalities.loggingIn.ServiceInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    internal class GoogleJwtValidationParamsGetter : IJwtValidationParamsGetter
    {
        private readonly IConfiguration _configuration;

        public GoogleJwtValidationParamsGetter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TokenValidationParameters FillParameters(JsonWebKeySet keySet)
        {
            return new TokenValidationParameters()
            {
                ValidIssuer = _configuration["GoogleToken:Issuer"],
                ValidAudience = _configuration["GoogleToken:Audience"],
                IssuerSigningKeys = keySet.Keys,
            };
        }
    }
}
