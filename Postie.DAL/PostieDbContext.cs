using Microsoft.EntityFrameworkCore;
using Postie.DAL.Entities;

namespace Postie.DAL
{
    public class PostieDbContext : DbContext
    {
        public PostieDbContext(DbContextOptions<PostieDbContext> options)
            : base(options)
        {

        }

        public DbSet<PostEntity> Posts { get; set; }
    }
}
