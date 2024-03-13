using AuthFrontend.functionalities.loggingIn.DTOs;
using AuthFrontend.functionalities.loggingIn.Repositories;
using AuthFrontend.seeds;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace AuthFrontend.functionalities.loggingIn.JwtStuff
{
    public class AuthTokenProvider(IAuthRepo authRepo, IConfiguration configuration, Random random)
    {
        private readonly IAuthRepo _authRepo = authRepo;
        private readonly IConfiguration _configuration = configuration;
        private readonly Random _random = random;

        public async Task<TokensDto?> MakeAccessToken(UserInfoDto userInfo)
        {
            var userId = await _authRepo.GetUserByEmail(userInfo.Email);
            if (!userId.HasValue)
                userId = await _authRepo.CreateUser(userInfo);

            if (!userId.HasValue)
                return null;

            #region Access
            JwtSecurityToken token;
            {
                var rsa = RSA.Create();
                rsa.ImportFromPem(_configuration["SigningCredentials:Auth:PrivateRSAKey"]);
                var securitykey = new RsaSecurityKey(rsa)
                {
                    KeyId = _configuration["SigningCredentials:JWK:kid"]
                };

                var desc = new SecurityTokenDescriptor()
                {
                    Expires = DateTimeOffset.Now.AddMinutes(int.Parse(_configuration["SigningCredentials:Auth:ExpiresAddMinutes"]!)).DateTime,
                    SigningCredentials = new SigningCredentials(securitykey, _configuration["SigningCredentials:Auth:JWK:alg"]),
                    Audience = _configuration["SigningCredentials:Auth:Audience"],
                    Issuer = _configuration["SigningCredentials:Auth:Issuer"],
                    NotBefore = DateTimeOffset.Now.DateTime,
                    IssuedAt = DateTimeOffset.Now.DateTime,
                    TokenType = "JWT",
                    AdditionalInnerHeaderClaims = new Dictionary<string, object>() { { "jti", Guid.NewGuid().ToString() } },
                    Claims = new Dictionary<string, object>() {
                    { "UserId", userId.Value.ToString() },
                    { SeedAuthClaimNames.Email, userInfo.Email },
                    { SeedAuthClaimNames.Username, userInfo.UserName },
                    { "Purpose", "Access" }
                }
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                token = tokenHandler.CreateJwtSecurityToken(desc);
            }
            #endregion

            #region Refresh
            JwtSecurityToken refreshToken;
            {
                var refreshRsa = RSA.Create();
                refreshRsa.ImportFromPem(_configuration["SigningCredentials:Refresh:PrivateRSAKey"]);
                var refreshSecuritykey = new RsaSecurityKey(refreshRsa)
                {
                    KeyId = _configuration["SigningCredentials:Refresh:JWK:kid"]
                };

                var refreshDesc = new SecurityTokenDescriptor()
                {
                    Expires = DateTimeOffset.Now.AddMinutes(int.Parse(_configuration["SigningCredentials:Refresh:ExpiresAddMinutes"]!)).DateTime,
                    SigningCredentials = new SigningCredentials(refreshSecuritykey, _configuration["SigningCredentials:Refresh:JWK:alg"]),
                    Audience = _configuration["SigningCredentials:Refresh:Audience"],
                    Issuer = _configuration["SigningCredentials:Refresh:Issuer"],
                    NotBefore = DateTimeOffset.Now.AddMinutes(int.Parse(_configuration["SigningCredentials:Refresh:NotBeforeAddMinutes"]!)).DateTime,
                    IssuedAt = DateTimeOffset.Now.DateTime,
                    TokenType = "JWT",
                    AdditionalInnerHeaderClaims = new Dictionary<string, object>() { { "jti", Guid.NewGuid().ToString() } },
                    Claims = new Dictionary<string, object>() {
                    { "UserId", userId.Value.ToString() },
                    { SeedAuthClaimNames.Email, userInfo.Email },
                    { SeedAuthClaimNames.Username, userInfo.UserName },
                    { "Purpose", "Refresh" }
                }
                };
                var refreshTokenHandler = new JwtSecurityTokenHandler();
                refreshToken = refreshTokenHandler.CreateJwtSecurityToken(refreshDesc);
            }
            #endregion

            #region SaveRefresh

            var stringRefreshToken = refreshToken.RawData;
            var salt = RandomString();
            var hashedRefreshToken = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(stringRefreshToken + salt)));
            var res = await _authRepo.AddRefreshToken(new RefreshTokenDto
            {
                AuthUserId = userId.Value,
                Expire = new DateTimeOffset(refreshToken.ValidTo).ToUnixTimeMilliseconds(),
                HashedToken = hashedRefreshToken,
                JTI = Guid.Parse((string)refreshToken.Header["jti"]),
                Salt = salt
            });
            #endregion

            if (!res)
                return null;

            return new TokensDto
            {
                AccessToken = token.RawData,
                RefreshToken = refreshToken.RawData
            };
        }

        private string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 20)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
