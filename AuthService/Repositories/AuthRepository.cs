using AutoMapper;
using Postie.DAL;
using Postie.DAL.Entities;
using Postie.Interfaces;
using Postie.Models;

namespace AuthService.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly PostieDbContext _context;
        private readonly IMapper _mapper;

        public AuthRepository(PostieDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Account Get(string email)
        {
            var entity = _context.Accounts.Where(x => x.Email == email).FirstOrDefault();

            return entity == null ? new Account() : AccountMapper(entity);
        }

        private Account AccountMapper(AccountEntity entity)
        {
            return _mapper.Map<Account>(entity);
        }
    }
}
