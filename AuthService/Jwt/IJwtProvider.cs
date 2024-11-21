using Postie.Models;

namespace AuthService.Jwt
{
    public interface IJwtProvider
    {
        string GenerateToken(Account account);
    }
}