using Postie.Models;

namespace Postie.Interfaces
{
    public interface IPostService
    {
        ICollection<Post> List();
        Post Get(Guid id);
        Guid Update(Guid id, string text);
        Guid Create(string text);
        bool Delete(Guid id);
    }
}
