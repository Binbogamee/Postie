using Postie.Models;

namespace Postie.Interfaces
{
    public interface IPostRepository
    {
        ICollection<Post> List();
        Post Get(Guid id);
        void Update(Post post);
        void Create(Post post);
        bool Delete(Guid id);
    }
}
