using Microsoft.EntityFrameworkCore;
using Postie.DAL;
using Postie.DAL.Entities;
using Postie.Interfaces;
using Postie.Models;

namespace PostService.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly PostieDbContext _context;
        public PostRepository(PostieDbContext context)
        {
            _context = context;
        }

        public bool Create(Post post)
        {
            try
            {
                _context.Add(PostMapper(post));
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(Guid id)
        {
            try
            {
                var deleted = _context.Posts.Where(x => x.Id == id).ExecuteDelete();
                _context.SaveChanges();
                return deleted != 0;
            }
            catch
            {
                return false;
            }
        }

        public Post Get(Guid id)
        {
            try
            {
                var entity = _context.Posts.AsNoTracking().FirstOrDefault(x => x.Id == id);

                if (entity == null)
                {
                    return new Post();
                }
                return PostMapper(entity);
            }
            catch
            {
                return new Post();
            }

        }

        public ICollection<Post> List()
        {
            try
            {
                var entities = _context.Posts.AsNoTracking().ToList();
                return entities.Select(x => PostMapper(x)).ToList();
            }
            catch
            {
                return new List<Post>();
            }
        }

        public bool Update(Post post)
        {
            try
            {
                var entity = PostMapper(post);
                _context.Posts.Update(entity);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

            
        }

        private Post PostMapper(PostEntity entity)
        {
            return new Post(entity.Text, entity.Id, entity.CreatedBy, entity.ModifiedBy);
        }

        private PostEntity PostMapper(Post post)
        {
            return new PostEntity()
            {
                Id = post.Id,
                Text = post.Text,
                CreatedBy = post.CreatedBy,
                ModifiedBy = post.ModifiedBy
            };
        }
    }
}
