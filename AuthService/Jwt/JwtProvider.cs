using AuthHelper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Postie.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Jwt
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _options;
        public JwtProvider(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }
        public string GenerateToken(Account account)
        {
            Claim[] claims = [
                new("accountId", account.Id.ToString())
            ];

            var signingCredantials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                
                signingCredentials: signingCredantials,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpiratesMinutes));

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenValue;
        }
    }
}
