using AuthFrontend.functionalities.loggingIn.ServiceInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    internal class GoogleJwtKeySetGetter : IJwtKeySetGetter
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GoogleJwtKeySetGetter(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<JsonWebKeySet?> GetKeySet()
        {
            var keysResponse = await _httpClient.GetAsync(_configuration["GoogleToken:KeysUrl"]);

            if (keysResponse == null)
                return null;

            try { keysResponse.EnsureSuccessStatusCode(); }
            catch (Exception) { return null; }

            var keysString = await keysResponse.Content.ReadAsStringAsync();
            return new JsonWebKeySet(keysString);
        }
    }
}
