using Postie.Models;

namespace Postie.Interfaces
{
    public interface IAccountService
    {
        List<Account> List();
        Account Get(Guid id);
        (Guid id, string error) Create(string username, string email, string password);
        bool Delete(Guid id);
        (Guid id, string error) Update(Guid id, string username, string email);
        string ChangePassword(Guid id, string oldPassword, string newPassword);
    }
}
