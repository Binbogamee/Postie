using Postie.Interfaces;
using Postie.Models;
using System.Text;
using LogLevel = NLog.LogLevel;

namespace AccountService.Services
{
    public class AccountService : IAccountService
    {
        private const int Max_Username_Length = 50;
        private const int Max_Email_Length = 255;
        private const int Min_Password_Length = 8;

        private readonly IAccountRepository _accountRepository;
        private readonly ILoggingProducerService _loggingService;
        private readonly AccountManager _accountManager;

        public AccountService(IAccountRepository accountRepository, ILoggingProducerService loggingService)
        {
            _accountRepository = accountRepository;
            _loggingService = loggingService;
            _accountManager = AccountManager.Instance;
        }
        public Guid Create(string username, string email, string password)
        {
            username = username.Trim();
            email = email.Trim();
            password = password.Trim();

            var validated = ValidateUserData(username, email);
            var passwordCheck = ValidatePassword(password);
            if (validated != string.Empty || passwordCheck != string.Empty)
            {
                var sb = new StringBuilder();
                sb.Append(validated);
                sb.Append(passwordCheck);

                //_loggingService.SendLogMessage(LogLevel.Error, sb.ToString());
                return Guid.Empty;
            }

            var passwordhash = _accountManager.HashPassword(password);
            var account = new Account(username, email, passwordhash);

            if (_accountRepository.Create(account))
            {
                //_loggingService.SendLogMessage(LogLevel.Info, $"Account {account.Id} created.");
                return account.Id;
            }

            //_loggingService.SendLogMessage(LogLevel.Error, $"Failed to create account");
            return Guid.Empty;
        }

        public bool Delete(Guid id)
        {
            return _accountRepository.Delete(id);
        }

        public Account Get(Guid id)
        {
            return _accountRepository.Get(id);
        }

        public List<Account> List()
        {
            return _accountRepository.List().ToList();
        }

        public Guid Update(Guid id, string username, string email)
        {
            username = username.Trim();
            email = email.Trim();
            var validated = ValidateUserData(username, email);
            if (validated != string.Empty)
            {
                return Guid.Empty;
            }

            var oldAccount = Get(id);
            var account = new Account(username, email, oldAccount.PasswordHash, id);

            return _accountRepository.Update(account) ? account.Id : Guid.Empty;
        }

        public bool ChangePassword(Guid id, string oldPassword, string newPassword)
        {
            var oldAccount = Get(id);
            _accountManager.VerifyPassword(oldPassword, oldAccount.PasswordHash);
            var validated = ValidatePassword(newPassword);
            if (validated != string.Empty)
            {
                throw new Exception(validated);
            }

            var passwordHash = _accountManager.HashPassword(newPassword);
            var account = new Account(oldAccount.Username, oldAccount.Email, passwordHash, id);
            return _accountRepository.Update(account);
        }

        private string ValidateUserData(string username, string email)
        {
            if (username == string.Empty || email == string.Empty)
            {
                throw new Exception("Please fill in all fields.");
            }

            if (username.Length > Max_Username_Length)
            {
                throw new Exception("The username must be less than 50 characters.");
            }

            var accounts = List();
            if (accounts.Select(x => x.Username).Contains(username))
            {
                throw new Exception("This username is already taken.");
            }

            if (email.Length > Max_Email_Length)
            {
                throw new Exception("The email must be less than 255 characters.");
            }

            if (accounts.Select(x => x.Email).Contains(username))
            {
                throw new Exception("An account has already been registered to this email.");
            }

            if (email.EndsWith("."))
            {
                throw new Exception("Invalid email.");
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    throw new Exception();
                }
            }
            catch
            {
                throw new Exception("Invalid email.");
            }

            return string.Empty;
        }

        private string ValidatePassword(string password)
        {
            if (password.Length < Min_Password_Length)
            {
                throw new Exception("Password must contain more than 8 characters.");
            }

            if (!password.Any(char.IsDigit) || !password.Any(char.IsUpper))
            {
                throw new Exception("The password must contain at least one number and a capital letter.");
            }

            return string.Empty;
        }
    }
}
