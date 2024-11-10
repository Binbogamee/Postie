using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postie.DAL.Entities;

namespace Postie.DAL.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<PostEntity>
    {
        public void Configure(EntityTypeBuilder<PostEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Text)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.CreatedBy)
                .IsRequired();

            builder.Property(x => x.ModifiedBy);
        }
    }
}
