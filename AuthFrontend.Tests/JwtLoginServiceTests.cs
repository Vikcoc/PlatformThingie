using AuthFrontend.functionalities.loggingIn.ServiceInterfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace AuthFrontend.Tests
{
    public abstract class JwtLoginServiceTests(IJwtLogValidatorService logInService)
    {
        // For testing purposes
        // https://travistidwell.com/jsencrypt/demo/
        // https://pem2jwk.vercel.app/

        protected readonly IJwtLogValidatorService _logInService = logInService;

        protected class HardcodedParams : IJwtValidationParamsGetter
        {
            public Task<TokenValidationParameters?> FillParameters()
            {
                var keySet = new JsonWebKeySet();

                keySet.Keys.Add(new JsonWebKey()
                {
                    N = "c1wLj2jMOAuUqRkF9PNkJVx4vuYE-CcAcarXfgrHtM0PBgKOArZYrEk0u1Q9Ki0fYRf0Qwhg_AJpQkzrB4cr-PbH-rCYn-FUZubFe5mYz5WNr8pH5BMt6AWdtFxVxAC93mfS6ebr30Y9NvfjkJI7es0hvYaG6czApTiIATwXFGk",
                    Use = "sig",
                    Kty = "RSA",
                    Alg = "RS256",
                    E = "AQAB",
                    Kid = "76a269304c3e91250c9a33932cb22"
                });

                return Task.FromResult<TokenValidationParameters?>(new TokenValidationParameters()
                {
                    ValidIssuer = "me",
                    ValidAudience = "me",
                    IssuerSigningKeys = keySet.Keys,
                });
            }
        }

        [Fact]
        public void ValidateToken_BadSignatureAndExpired_ReturnInvalid()
        {
            var result = _logInService.ValidateToken("eyJhbGciOiJSUzI1NiIsImtpZCI6Ijc2YTI2OTMwNGMzZTkxMjUwYzlhMzM5MzJjYjIyIiwidHlwIjoiSldUIn0.eyJuYmYiOjE3MDk3MjcyOTMsImV4cCI6MTcwOTczMDg5MywiaWF0IjoxNzA5NzI3MjkzLCJpc3MiOiJtZSIsImF1ZCI6Im1lIn0.MXmjoe2pXQ6jfcmnEF2kAajfpcWGtzlrCRKsUDF2EwALzwR_pbcgTmG3c4-firRbrbiWTH3psnis2HDvViQzPsNAlejM_xR-cs21TuFoFYetDTx0SOJcmCd4OQKWPLP7co9l6eE-s7I3zpJiQEFy3TXnfZznc3HWc1G5J6wefFU").Result;

            Assert.Null(result);
        }

        [Fact]
        public void ValidateToken_ValidToken_ReturnValid()
        {
            var result = _logInService.ValidateToken(MakeTokin());

            Assert.NotNull(result);
        }

        protected string MakeTokin()
        {
            var privateKey = "-----BEGIN RSA PRIVATE KEY-----\r\nMIICWgIBAAKBgHNcC49ozDgLlKkZBfTzZCVceL7mBPgnAHGq134Kx7TNDwYCjgK2\r\nWKxJNLtUPSotH2EX9EMIYPwCaUJM6weHK/j2x/qwmJ/hVGbmxXuZmM+Vja/KR+QT\r\nLegFnbRcVcQAvd5n0unm699GPTb345CSO3rNIb2GhunMwKU4iAE8FxRpAgMBAAEC\r\ngYAIfyAO2P4pppi1d7VbnE2k/tGZ5eE50c2Tkay7iNm6tFvF6oxBIOMZFW8/2O2E\r\nXLxx70y7XzOdfCP6kSXCcS9I1ac2w2NP1Tn/01sgMci5YwbxacB0vkSVdXD2jWyE\r\nHGmat/A5wXeuBTx+IbZXeN5jRggJvp31tBPgVhTT2KjMAQJBAOXrKjil7PoennZs\r\nIPMWO8V4UPPnMVcTP8n3nt+tfRgxjkq0qFUkEt8Uu3LOFH5Il8jaIuV82CPXdRT3\r\n91t+foECQQCAchX3LNBBiaDosCSqYszkRbcNAr82cEUpipy0qJ6vZDYmnUqMdDB8\r\nBta6rV3+9E+pyRMJbYuw/pecysCOn3HpAkBZ7D/3J+4nZRJU/rVkXa3C7eu7eRCz\r\nHbQ1KcPZd+EVNUyRq1aq9hDrbxBhNniNa5bx118OomVmnz4LlXAJONABAkBohu8z\r\nMEfmHa9RzQ55jl+5ILa/os2i6qiODtxJ9LL2frHGqNgjBubx98rFLhLBSWaPADA6\r\nyugUSItvMFyVdFIJAkA/FYS/xl/JZnRhRLNdR+toUhyNwE65xsv5dtGgAo5LUQCJ\r\nJMZnfuRRdy9ZJMsvyXgVytbnAFojAVJEFlip508O\r\n-----END RSA PRIVATE KEY-----";
            var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);
            var securitykey = new RsaSecurityKey(rsa);
            securitykey.KeyId = "76a269304c3e91250c9a33932cb22";

            var desc = new SecurityTokenDescriptor()
            {
                Expires = DateTimeOffset.Now.AddHours(1).DateTime,
                SigningCredentials = new SigningCredentials(securitykey, SecurityAlgorithms.RsaSha256),
                Audience = "me",
                Issuer = "me",

            };
            var tokenHandler = new JwtSecurityTokenHandler
            {
                SetDefaultTimesOnTokenCreation = true
            };
            var token = tokenHandler.CreateJwtSecurityToken(desc);

            return token.RawData;
        }
    }
}
