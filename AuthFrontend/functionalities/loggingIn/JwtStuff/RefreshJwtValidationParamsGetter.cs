using AuthFrontend.functionalities.loggingIn.ServiceInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    internal class RefreshJwtValidationParamsGetter(IConfiguration configuration) : IJwtValidationParamsGetter
    {
        private readonly IConfiguration _configuration = configuration;

        public Task<TokenValidationParameters?> FillParameters()
        {
            var key = _configuration.GetSection("SigningCredentials:Refresh:JWK").Get<JsonWebKey>();
            var keySet = new JsonWebKeySet();
            keySet.Keys.Add(key);

            var parameters = new TokenValidationParameters()
            {
                ValidIssuer = _configuration["SigningCredentials:Refresh:Issuer"],
                ValidAudience = _configuration["SigningCredentials:Refresh:Audience"],
                IssuerSigningKeys = keySet.Keys,
            };

            return Task.FromResult<TokenValidationParameters?>(parameters);
        }
    }
}
