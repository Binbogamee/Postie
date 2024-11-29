using Postie.Models;

namespace Postie.Interfaces
{
    public interface IAccountService
    {
        Result<ICollection<Account>> List();
        Result<Account> Get(Guid id);
        Result<Guid> Create(string username, string email, string password);
        Result<bool> Delete(Guid requesterId,Guid id);
        Result<Guid> Update(Guid requesterId,Guid id, string username, string email);
        Result<string> ChangePassword(Guid requesterId,Guid id, string oldPassword, string newPassword);
    }
}
