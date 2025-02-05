using Postie.Dtos;
using Postie.Models;

namespace Postie.Interfaces
{
    public interface IPostService
    {
        Result<ICollection<CreatedPostDto>> List();
        Result<ICollection<CreatedPostDto>> ListByAccountId(Guid accountId);
        Result<CreatedPostDto> Get(Guid id);
        Result<Guid> Update(Guid id, Guid requesterId, RequestPostDto request);
        Result<Guid> Create(Guid requesterId, RequestPostDto request);
        Result<bool> Delete(Guid id, Guid requesterId);
    }
}
