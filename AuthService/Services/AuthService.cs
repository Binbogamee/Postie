using AuthService.Jwt;
using Postie.Infrastructure;
using Postie.Interfaces;

namespace AuthService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly PasswordHashHelper _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public AuthService(IAuthRepository authRepository, IJwtProvider jwtProvider)
        {
            _jwtProvider = jwtProvider;
            _authRepository = authRepository;
            _passwordHasher = PasswordHashHelper.Instance;
        }
        public string Login(string email, string password)
        {
            var user = _authRepository.Get(email);
            _passwordHasher.VerifyPassword(password, user.PasswordHash);

            var token = _jwtProvider.GenerateToken(user);

            return token;
        }
    }
}
