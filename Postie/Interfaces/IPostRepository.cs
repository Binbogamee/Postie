using Postie.Models;

namespace Postie.Interfaces
{
    public interface IPostRepository
    {
        ICollection<Post> List();
        ICollection<Post> ListByAccountId(Guid accountId);
        Post Get(Guid id);
        void Update(Post post);
        void Create(Post post);
        bool Delete(Guid id);
    }
}
