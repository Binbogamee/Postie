using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Postie.DAL.Entities;

namespace Postie.DAL.Configurations
{
    internal class AccountConfiguration : IEntityTypeConfiguration<AccountEntity>
    {
        public void Configure(EntityTypeBuilder<AccountEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Username)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.PasswordHash)
                .IsRequired();
        }
    }
}
