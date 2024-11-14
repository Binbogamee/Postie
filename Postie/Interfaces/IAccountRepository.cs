using Postie.Models;

namespace Postie.Interfaces
{
    public interface IAccountRepository
    {
        ICollection<Account> List();
        Account Get(Guid id);
        bool Update(Account account);
        bool Create(Account account);
        bool Delete(Guid id);
    }
}
