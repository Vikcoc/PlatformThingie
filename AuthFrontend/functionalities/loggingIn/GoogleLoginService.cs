using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace AuthFrontend.functionalities.loggingIn
{
    internal sealed class GoogleLoginService : ILogInService
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly HttpClient _httpClient;

        public GoogleLoginService(JwtSecurityTokenHandler tokenHandler, HttpClient httpClient)
        {
            _tokenHandler = tokenHandler;
            _httpClient = httpClient;
        }

        public string MakeAccessToken(UserInfoDto userInfo)
        {
            return userInfo.Email + " " + userInfo.UserName;
        }

        public async Task<UserInfoDto?> ValidateToken(string token)
        {
            var keysResponse = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v3/certs");

            if(keysResponse == null) 
                return null;

            try { keysResponse.EnsureSuccessStatusCode(); }
            catch (Exception){return null;}

            var keysString = await keysResponse.Content.ReadAsStringAsync();
            var googleKeySet = new JsonWebKeySet(keysString);



            TokenValidationParameters parameters = new TokenValidationParameters()
            {
                ValidIssuer = "https://accounts.google.com",
                ValidAudience = "",
                IssuerSigningKeys = googleKeySet.Keys,
            };

            JwtSecurityToken parsedToken;
            try
            {
                _tokenHandler.ValidateToken(token, parameters, out var abstractToken);
                parsedToken = (JwtSecurityToken)abstractToken;
            }
            catch (Exception) { return null;}

            var props = parsedToken.Claims.ToDictionary(x => x.Type, x => x.Value);

            return new UserInfoDto
            {
                Email = props["email"],
                UserName = props["name"],
            };
        }
    }
}
