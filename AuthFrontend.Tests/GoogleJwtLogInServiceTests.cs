using AuthFrontend.functionalities.loggingIn;
using System.IdentityModel.Tokens.Jwt;

namespace AuthFrontend.Tests
{
    public class GoogleJwtLogInServiceTests : JwtLoginServiceTests
    {
        public GoogleJwtLogInServiceTests() : base(new GoogleJwtLoginService(new JwtSecurityTokenHandler(), new HardcodedKey(), new HardcodedParams()))
        { }
    }
}
