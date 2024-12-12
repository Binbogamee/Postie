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

        public void Create(Account account)
        {
            _context.Add(AccountMapper(account));
            _context.SaveChanges();
            _context.ChangeTracker.Clear();
        }

        public bool Delete(Guid id)
        {
            var deleted = _context.Accounts.Where(x => x.Id == id).ExecuteDelete();
            _context.SaveChanges();
            return deleted != 0;
        }

        public Account Get(Guid id)
        {
            var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.Id == id);
            return account == null ? new Account() : AccountMapper(account);
        }

        public ICollection<Account> List()
        {
            var entities = _context.Accounts.AsNoTracking().ToList();
            return entities.Select(x => AccountMapper(x)).ToList();
        }

        public void Update(Account account)
        {
            var entity = AccountMapper(account);
            
            _context.Accounts.Update(entity);
            _context.SaveChanges();
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
