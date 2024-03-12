using AuthFrontend.functionalities.loggingIn.ServiceInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    internal class GoogleJwtValidationParamsGetter(IConfiguration configuration, HttpClient httpClient) : IJwtValidationParamsGetter
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly HttpClient _httpClient = httpClient;

        public async Task<TokenValidationParameters?> FillParameters()
        {
            var keysResponse = await _httpClient.GetAsync(_configuration["GoogleToken:KeysUrl"]);

            if (keysResponse == null)
                return null;

            try { keysResponse.EnsureSuccessStatusCode(); }
            catch (Exception) { return null; }

            var keysString = await keysResponse.Content.ReadAsStringAsync();
            var keySet = new JsonWebKeySet(keysString);

            return new TokenValidationParameters()
            {
                ValidIssuer = _configuration["GoogleToken:Issuer"],
                ValidAudience = _configuration["GoogleToken:Audience"],
                IssuerSigningKeys = keySet.Keys,
            };
        }
    }
}
