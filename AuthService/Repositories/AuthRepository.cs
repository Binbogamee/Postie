using Postie.DAL;
using Postie.DAL.Entities;
using Postie.Interfaces;
using Postie.Models;

namespace AuthService.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly PostieDbContext _context;

        public AuthRepository(PostieDbContext context)
        {
            _context = context;
        }
        public Account Get(string email)
        {
            var entity = _context.Accounts.Where(x => x.Email == email).FirstOrDefault();

            return entity == null ? new Account() : AccountMapper(entity);
        }

        private Account AccountMapper(AccountEntity entity)
        {
            return new Account(entity.Username, entity.Email, entity.PasswordHash, entity.Id);
        }
    }
}
