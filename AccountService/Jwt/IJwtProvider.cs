using Postie.Models;

namespace AccountService.Jwt
{
    public interface IJwtProvider
    {
        string GenerateToken(Account account);
    }
}