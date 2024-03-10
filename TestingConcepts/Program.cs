using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

string MakeTokin()
{
    var privateKey = "-----BEGIN RSA PRIVATE KEY-----\r\nMIICWgIBAAKBgHNcC49ozDgLlKkZBfTzZCVceL7mBPgnAHGq134Kx7TNDwYCjgK2\r\nWKxJNLtUPSotH2EX9EMIYPwCaUJM6weHK/j2x/qwmJ/hVGbmxXuZmM+Vja/KR+QT\r\nLegFnbRcVcQAvd5n0unm699GPTb345CSO3rNIb2GhunMwKU4iAE8FxRpAgMBAAEC\r\ngYAIfyAO2P4pppi1d7VbnE2k/tGZ5eE50c2Tkay7iNm6tFvF6oxBIOMZFW8/2O2E\r\nXLxx70y7XzOdfCP6kSXCcS9I1ac2w2NP1Tn/01sgMci5YwbxacB0vkSVdXD2jWyE\r\nHGmat/A5wXeuBTx+IbZXeN5jRggJvp31tBPgVhTT2KjMAQJBAOXrKjil7PoennZs\r\nIPMWO8V4UPPnMVcTP8n3nt+tfRgxjkq0qFUkEt8Uu3LOFH5Il8jaIuV82CPXdRT3\r\n91t+foECQQCAchX3LNBBiaDosCSqYszkRbcNAr82cEUpipy0qJ6vZDYmnUqMdDB8\r\nBta6rV3+9E+pyRMJbYuw/pecysCOn3HpAkBZ7D/3J+4nZRJU/rVkXa3C7eu7eRCz\r\nHbQ1KcPZd+EVNUyRq1aq9hDrbxBhNniNa5bx118OomVmnz4LlXAJONABAkBohu8z\r\nMEfmHa9RzQ55jl+5ILa/os2i6qiODtxJ9LL2frHGqNgjBubx98rFLhLBSWaPADA6\r\nyugUSItvMFyVdFIJAkA/FYS/xl/JZnRhRLNdR+toUhyNwE65xsv5dtGgAo5LUQCJ\r\nJMZnfuRRdy9ZJMsvyXgVytbnAFojAVJEFlip508O\r\n-----END RSA PRIVATE KEY-----";
    //var pubKey = "MIGeMA0GCSqGSIb3DQEBAQUAA4GMADCBiAKBgHNcC49ozDgLlKkZBfTzZCVceL7m\r\nBPgnAHGq134Kx7TNDwYCjgK2WKxJNLtUPSotH2EX9EMIYPwCaUJM6weHK/j2x/qw\r\nmJ/hVGbmxXuZmM+Vja/KR+QTLegFnbRcVcQAvd5n0unm699GPTb345CSO3rNIb2G\r\nhunMwKU4iAE8FxRpAgMBAAE=";
    var rsa = RSA.Create();
    rsa.ImportFromPem(privateKey);
    var securitykey = new RsaSecurityKey(rsa);
    securitykey.KeyId = "76a269304c3e91250c9a33932cb22";

    var desc = new SecurityTokenDescriptor() 
    {
        Expires = DateTimeOffset.Now.AddHours(1).DateTime,
        SigningCredentials = new SigningCredentials(securitykey, SecurityAlgorithms.RsaSha256) ,
        Audience = "me",
        Issuer = "me",
        TokenType = "Panamera",
        AdditionalInnerHeaderClaims = new Dictionary<string, object>() { { "jti", "dawdadawd" } },
        
    };

    var tokenHandler = new JwtSecurityTokenHandler
    {
        SetDefaultTimesOnTokenCreation = true
    };
    var token = tokenHandler.CreateJwtSecurityToken(desc);

    return token.RawData;
}


Console.WriteLine(MakeTokin());