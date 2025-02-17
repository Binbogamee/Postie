﻿using AutoMapper;
using Postie.Dtos;
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
        private readonly PasswordHashHelper _accountManager;
        private readonly IMapper _mapper;

        public AccountService(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _accountManager = PasswordHashHelper.Instance;
            _mapper = mapper;
        }
        public Result<Guid> Create(NewAccountRequest request)
        {
            var username = request.Username.Trim();
            var email = request.Email.Trim();
            var password = request.Password.Trim();

            var validated = ValidateUserData(username, email);
            var passwordCheck = ValidatePassword(password);
            if (validated != string.Empty || passwordCheck != string.Empty)
            {
                var sb = new StringBuilder();
                sb.Append(validated);
                sb.Append(passwordCheck);

                return Result<Guid>.Failure(ErrorType.ValidationError, sb.ToString());
            }

            var passwordhash = _accountManager.HashPassword(password);
            var account = new Account(username, email, passwordhash);

            _accountRepository.Create(account);
            return Result<Guid>.Success(account.Id);
        }

        public Result<bool> Delete(Guid requesterId, Guid id)
        {
            if (!IsAuthorized(requesterId, id))
            {
                return Result<bool>.Failure(ErrorType.AccessDenied);
            }

            return Result<bool>.Success(_accountRepository.Delete(id));
        }

        public Result<AccountDto> Get(Guid id)
        {
            var account = _accountRepository.Get(id);
            return Result<AccountDto>.Success(_mapper.Map<AccountDto>(account));
        }

        public Result<ICollection<AccountDto>> List()
        {
            var accounts = _accountRepository.List();
            return Result<ICollection<AccountDto>>.Success(_mapper.Map<List<AccountDto>>(accounts));
        }

        public Result<Guid> Update(Guid requesterId, AccountDto dto)
        {
            var accountFromDto = _mapper.Map<AccountDto>(dto);
            if (!IsAuthorized(requesterId, accountFromDto.Id))
            {
                return Result<Guid>.Failure(ErrorType.AccessDenied);
            }

            var validated = ValidateUserData(accountFromDto.Username, accountFromDto.Email, false);
            if (validated != string.Empty)
            {
                return Result<Guid>.Failure(ErrorType.ValidationError, validated);
            }

            var oldAccount = _accountRepository.Get(accountFromDto.Id);
            if (oldAccount.Id == Guid.Empty)
            {
                return Result<Guid>.Failure(ErrorType.NotFound);
            }
            var account = new Account(accountFromDto.Username, accountFromDto.Email, oldAccount.PasswordHash, accountFromDto.Id);
            _accountRepository.Update(account);
            return Result<Guid>.Success(account.Id);
        }

        public Result<string> ChangePassword(Guid requesterId, Guid id, string oldPassword, string newPassword)
        {
            if (!IsAuthorized(requesterId, id))
            {
                return Result<string>.Failure(ErrorType.AccessDenied);
            }

            var oldAccount = _accountRepository.Get(id);
            if (oldAccount.Id == Guid.Empty)
            {
                return Result<string>.Failure(ErrorType.NotFound, Messages.AccountNotFound);
            }

            if (!_accountManager.VerifyPassword(oldPassword, oldAccount.PasswordHash))
            {
                return Result<string>.Failure(ErrorType.ValidationError, Messages.WrongEmailOrPassword);
            }
            var validated = ValidatePassword(newPassword);
            if (validated != string.Empty)
            {
                return Result<string>.Failure(ErrorType.ValidationError, validated);
            }

            var passwordHash = _accountManager.HashPassword(newPassword);
            var account = new Account(oldAccount.Username, oldAccount.Email, passwordHash, id);
            _accountRepository.Update(account);
            return Result<string>.Success(string.Empty);
        }

        private string ValidateUserData(string username, string email, bool IsNew = true)
        {
            if (username == string.Empty || email == string.Empty)
            {
                return Messages.FillFields;
            }

            if (username.Length > Max_Username_Length)
            {
                return Messages.UsernameIsTooLong;
            }

            if (IsNew)
            {
                var accounts = _accountRepository.List();
                if (accounts.Select(x => x.Username).Contains(username))
                {
                    return Messages.UsernameIsTaken_;
                }

                if (accounts.Select(x => x.Email).Contains(username))
                {
                    return Messages.EmailIsTaken;
                }
            }

            if (email.Length > Max_Email_Length)
            {
                return Messages.EmailIsTooLong;
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

        private bool IsAuthorized(Guid requesterId, Guid targerId)
        {
            return requesterId == targerId;
        }
    }
}
