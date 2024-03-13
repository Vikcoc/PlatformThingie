using AuthFrontend.functionalities.loggingIn.JwtStuff;
using System.IdentityModel.Tokens.Jwt;

namespace AuthFrontend.Tests
{
    public class RefreshJwtValidationServiceTests : JwtLoginServiceTests
    {
        public RefreshJwtValidationServiceTests() : base(new RefreshJwtValidationService(null!, new JwtSecurityTokenHandler(), new HardcodedParams()))
        {
        }
    }
}
