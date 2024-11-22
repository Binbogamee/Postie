using Postie.Models;

namespace Postie.Interfaces
{
    public interface IAccountRepository
    {
        ICollection<Account> List();
        Account Get(Guid id);
        void Update(Account account);
        void Create(Account account);
        bool Delete(Guid id);
    }
}
