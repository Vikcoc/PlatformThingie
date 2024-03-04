using AuthFrontend.functionalities.loggingIn;
using AuthFrontend.functionalities.loggingIn.JwtStuff;
using Microsoft.IdentityModel.Tokens;

namespace AuthFrontend.Tests
{
    public abstract class LoginServiceTests
    {
        protected readonly IJwtLogInService _logInService;

        protected LoginServiceTests(IJwtLogInService logInService)
        {
            _logInService = logInService;
        }

        protected class HardcodedKey : IJwtKeySetGetter
        {
            public Task<JsonWebKeySet?> GetKeySet() 
                => Task.FromResult<JsonWebKeySet?>(new JsonWebKeySet());
        }

        protected class HardcodedParams : IJwtValidationParamsGetter
        {
            public TokenValidationParameters FillParameters(JsonWebKeySet keySet) => new TokenValidationParameters()
            {
                ValidIssuer = "yes",
                ValidAudience = "yes",
                IssuerSigningKeys = keySet.Keys,
            };;
        }

        [Fact]
        public void ValidateToken_ValidToken_ReturnValid()
        {
            var result = _logInService.ValidateToken("asdienaodn", new HardcodedKey(), new HardcodedParams()).Result;

            Assert.NotNull(result);
        }
    }
}
