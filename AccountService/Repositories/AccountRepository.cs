using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Postie.DAL;
using Postie.DAL.Entities;
using Postie.Interfaces;
using Postie.Models;

namespace AccountService.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly PostieDbContext _context;
        private readonly IMapper _mapper;

        public AccountRepository(PostieDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            _context.ChangeTracker.Clear();
        }

        private Account AccountMapper(AccountEntity entity)
        {
            return _mapper.Map<Account>(entity);
        }

        private AccountEntity AccountMapper(Account account)
        {
            return _mapper.Map<AccountEntity>(account);
        }
    }
}
