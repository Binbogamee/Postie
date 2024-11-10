using Postie.Models;

namespace Postie.Interfaces
{
    public interface IPostRepository
    {
        ICollection<Post> List();
        Post Get(Guid id);
        bool Update(Post post);
        bool Create(Post post);
        bool Delete(Guid id);
    }
}
