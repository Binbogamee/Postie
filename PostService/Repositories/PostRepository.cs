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

        public void Create(Post post)
        {
            _context.Add(PostMapper(post));
            _context.SaveChanges();
        }

        public bool Delete(Guid id)
        {
            var deleted = _context.Posts.Where(x => x.Id == id).ExecuteDelete();
            _context.SaveChanges();
            return deleted != 0;
        }

        public Post Get(Guid id)
        {
            var entity = _context.Posts.AsNoTracking().FirstOrDefault(x => x.Id == id);

            if (entity == null)
            {
                return new Post();
            }

            return PostMapper(entity);
        }

        public ICollection<Post> List()
        {
            var entities = _context.Posts.AsNoTracking().ToList();
            return entities.Select(x => PostMapper(x)).ToList();
        }

        public ICollection<Post> ListByAccountId(Guid accountId)
        {
            var entities = _context.Posts.Where(x => x.AccountId == accountId).AsNoTracking().ToList();
            return entities.Select(x => PostMapper(x)).ToList();
        }

        public void Update(Post post)
        {
            var entity = PostMapper(post);
            _context.Posts.Update(entity);
            _context.SaveChanges();            
        }

        private Post PostMapper(PostEntity entity)
        {
            return new Post(entity.Text, entity.AccountId, entity.Id, entity.CreatedBy, entity.ModifiedBy);
        }

        private PostEntity PostMapper(Post post)
        {
            return new PostEntity()
            {
                Id = post.Id,
                AccountId = post.AccountId,
                Text = post.Text,
                CreatedBy = post.CreatedBy,
                ModifiedBy = post.ModifiedBy
            };
        }
    }
}
