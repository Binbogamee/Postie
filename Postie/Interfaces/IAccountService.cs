using Postie.Dtos;
using Postie.Models;

namespace Postie.Interfaces
{
    public interface IAccountService
    {
        Result<ICollection<AccountDto>> List();
        Result<AccountDto> Get(Guid id);
        Result<Guid> Create(NewAccountRequest request);
        Result<bool> Delete(Guid requesterId,Guid id);
        Result<Guid> Update(Guid requesterId, AccountDto request);
        Result<string> ChangePassword(Guid requesterId, Guid id, string oldPassword, string newPassword);
    }
}
