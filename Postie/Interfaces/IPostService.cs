using Postie.Models;

namespace Postie.Interfaces
{
    public interface IPostService
    {
        Result<ICollection<Post>> List();
        Result<ICollection<Post>> ListByAccountId(Guid accountId);
        Result<Post> Get(Guid id);
        Result<Guid> Update(Guid id, string text, Guid requesterId);
        Result<Guid> Create(Guid accountId, string text);
        Result<bool> Delete(Guid id, Guid requesterId);
    }
}
