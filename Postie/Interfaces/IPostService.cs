using Postie.Models;

namespace Postie.Interfaces
{
    public interface IPostService
    {
        ICollection<Post> List();
        Post Get(Guid id);
        (Guid id, string error) Update(Guid id, string text);
        (Guid id, string error) Create(string text);
        bool Delete(Guid id);
    }
}
