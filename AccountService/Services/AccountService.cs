using Postie.Infrastructure;
using Postie.Interfaces;
using Postie.Models;
using System.Text;
using Messages = Postie.Messages;

namespace AccountService.Services
{
    public class AccountService : IAccountService
    {
        private const int Max_Username_Length = 50;
        private const int Max_Email_Length = 255;
        private const int Min_Password_Length = 8;

        private readonly IAccountRepository _accountRepository;
        private readonly ILoggingProducerService _loggingService;
        private readonly PasswordHashHelper _accountManager;

        public AccountService(IAccountRepository accountRepository, ILoggingProducerService loggingService)
        {
            _accountRepository = accountRepository;
            _loggingService = loggingService;
            _accountManager = PasswordHashHelper.Instance;
        }
        public (Guid id, string error) Create(string username, string email, string password)
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

                return (Guid.Empty, sb.ToString());
            }

            var passwordhash = _accountManager.HashPassword(password);
            var account = new Account(username, email, passwordhash);

            _accountRepository.Create(account);
            return (account.Id, string.Empty);
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

        public (Guid id, string error) Update(Guid id, string username, string email)
        {
            username = username.Trim();
            email = email.Trim();
            var validated = ValidateUserData(username, email);
            if (validated != string.Empty)
            {
                return (Guid.Empty, validated);
            }

            var oldAccount = Get(id);
            if (oldAccount.Id == Guid.Empty)
            {
                return (Guid.Empty, Messages.AccountNotFound);
            }
            var account = new Account(username, email, oldAccount.PasswordHash, id);
            _accountRepository.Update(account);
            return (account.Id, string.Empty);
        }

        public string ChangePassword(Guid id, string oldPassword, string newPassword)
        {
            var oldAccount = Get(id);
            if (oldAccount.Id != Guid.Empty)
            {
                return Messages.AccountNotFound;
            }

            if (!_accountManager.VerifyPassword(oldPassword, oldAccount.PasswordHash))
            {
                return Messages.WrongEmailOrPassword;
            }
            var validated = ValidatePassword(newPassword);
            if (validated != string.Empty)
            {
                return validated;
            }

            var passwordHash = _accountManager.HashPassword(newPassword);
            var account = new Account(oldAccount.Username, oldAccount.Email, passwordHash, id);
            _accountRepository.Update(account);
            return string.Empty;
        }

        private string ValidateUserData(string username, string email)
        {
            if (username == string.Empty || email == string.Empty)
            {
                return Messages.FillFields;
            }

            if (username.Length > Max_Username_Length)
            {
                return Messages.UsernameIsTooLong;
            }

            var accounts = List();
            if (accounts.Select(x => x.Username).Contains(username))
            {
                return Messages.UsernameIsTaken_;
            }

            if (email.Length > Max_Email_Length)
            {
                return Messages.EmailIsTooLong;
            }

            if (accounts.Select(x => x.Email).Contains(username))
            {
                return Messages.EmailIsTaken;
            }

            if (email.EndsWith("."))
            {
                return Messages.InvalidEmail;
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
                return Messages.InvalidEmail;
            }

            return string.Empty;
        }

        private string ValidatePassword(string password)
        {
            if (password.Length < Min_Password_Length)
            {
                return Messages.PasswordIsTooShort;
            }

            if (!password.Any(char.IsDigit) || !password.Any(char.IsUpper))
            {
                return Messages.PasswordValidationError;
            }

            return string.Empty;
        }
    }
}
