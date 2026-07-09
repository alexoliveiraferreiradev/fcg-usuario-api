using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Users.Infrastructure.Persistence.Mapping
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
        }
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(p => p.Id);

            builder.OwnsOne(u => u.Name, n =>
            {
                n.Property(p => p.Value).HasColumnName("Name").IsRequired().HasMaxLength(50);
            });

            builder.OwnsOne(u => u.Email, e =>
            {
                e.Property(p => p.Value).HasColumnName("Email").IsRequired().HasMaxLength(100);
            });

            builder.OwnsOne(u => u.Password, s =>
            {
                s.Property(p => p.Hash).HasColumnName("Password").IsRequired().HasMaxLength(60);
            });

            builder.Property(u => u.DeactivationReason)
                .IsRequired(false);
        }
    }
}
