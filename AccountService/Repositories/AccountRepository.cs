using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Postie.DAL;
using Postie.DAL.Entities;
using Postie.Interfaces;
using Postie.Models;

namespace AccountService.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly PostieDbContext _context;

        public AccountRepository(PostieDbContext context)
        {
            _context = context;
        }

        public bool Create(Account account)
        {
            try
            {
                _context.Add(AccountMapper(account));
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(Guid id)
        {
            try
            {
                var deleted = _context.Accounts.Where(x => x.Id == id).ExecuteDelete();
                _context.SaveChanges();
                return deleted != 0;
            }
            catch
            {
                return false;
            }
        }

        public Account Get(Guid id)
        {
            try
            {
                var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.Id == id);
                return account == null ? new Account() : AccountMapper(account);
            }
            catch
            {
                return new Account();
            }
        }

        public ICollection<Account> List()
        {
            try
            {
                var entities = _context.Accounts.AsNoTracking().ToList();
                return entities.Select(x => AccountMapper(x)).ToList();
            }
            catch
            {
                return new List<Account>();
            }
        }

        public bool Update(Account account)
        {
            try
            {
                var entity = AccountMapper(account);
                _context.Accounts.Update(entity);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private Account AccountMapper(AccountEntity entity)
        {
            return new Account(entity.Username, entity.Email, entity.PasswordHash, entity.Id);
        }

        private AccountEntity AccountMapper(Account account)
        {
            return new AccountEntity()
            {
                Id = account.Id,
                Username = account.Username,
                Email = account.Email,
                PasswordHash = account.PasswordHash
            };
        }
    }
}
