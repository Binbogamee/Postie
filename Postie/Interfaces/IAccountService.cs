using Postie.Models;

namespace Postie.Interfaces
{
    public interface IAccountService
    {
        List<Account> List();
        Account Get(Guid id);
        Guid Create(string username, string email, string password);
        bool Delete(Guid id);
        Guid Update(Guid id, string username, string email);
        bool ChangePassword(Guid id, string oldPassword, string newPassword);
    }
}
