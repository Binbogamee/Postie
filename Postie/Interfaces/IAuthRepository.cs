using Postie.Models;

namespace Postie.Interfaces
{
    public interface IAuthRepository
    {
        Account Get(string email);
    }
}
