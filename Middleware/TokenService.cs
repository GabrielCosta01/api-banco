using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace api_banco.Middleware
{
    public class TokenService
    {
        public string GenerateToken(string userId, string secretKey)
        {
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyGenerator = new RNGCryptoServiceProvider();
            var key = new byte[16];
            keyGenerator.GetBytes(key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userId)
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        internal object GenerateToken(Guid id, string v)
        {
            throw new NotImplementedException();
        }
    }
}
