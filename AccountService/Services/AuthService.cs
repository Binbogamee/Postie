using AccountService.Jwt;
using Postie.Interfaces;

namespace AccountService.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly AccountManager _accountManager;
        private readonly IJwtProvider _jwtProvider;

        public AuthService(IAccountRepository accountRepository, IJwtProvider jwtProvider)
        {
            _jwtProvider = jwtProvider;
            _accountRepository = accountRepository;
            _accountManager = AccountManager.Instance;
        }
        public string Login(string email, string password)
        {
            var user = _accountRepository.Get(email);
            _accountManager.VerifyPassword(password, user.PasswordHash);

            var token = _jwtProvider.GenerateToken(user);

            return token;
        }
    }
}
