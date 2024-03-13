using AuthFrontend.functionalities.loggingIn.DTOs;
using AuthFrontend.functionalities.loggingIn.JwtStuff;
using AuthFrontend.functionalities.loggingIn.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace AuthFrontend.Tests
{
    public class RefreshJwtValidationServiceTests : JwtLoginServiceTests
    {
        public RefreshJwtValidationServiceTests() : base(new RefreshJwtValidationService(new HardcodedAuthRepo(), new JwtSecurityTokenHandler(), new HardcodedParams()))
        {
        }
        protected class HardcodedAuthRepo : IAuthRepo
        {
            public Task<bool> AddRefreshToken(RefreshTokenDto token)
            {
                throw new NotImplementedException();
            }

            public Task<Guid?> CreateUser(UserInfoDto user)
            {
                throw new NotImplementedException();
            }

            public Task<(string tokenHash, string tokenSalt)> GetTokenHashAndSalt(Guid jti)
                => Task.FromResult((Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(Token + "ABCDEF"))), "ABCDEF"));

            public Task<Guid?> GetUserByEmail(string email)
            {
                throw new NotImplementedException();
            }
        }
    }
}
