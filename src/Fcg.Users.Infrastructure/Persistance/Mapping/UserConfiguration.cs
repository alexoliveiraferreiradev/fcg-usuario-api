using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Users.Infrastructure.Persistance.Mapping
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

            var adminId = Guid.Parse("AEA0B4F3-D220-4C8D-ABA8-D868BE7CA593");
            builder.HasData(new
            {
                Id = adminId,
                IsActive = true,
                Role = UserRole.Admin,
                CreatedAt = new DateTime(2026, 5, 2),
                UpdatedAt = new DateTime(2026, 5, 2)
            });

            builder.OwnsOne(u => u.Name, n =>
            {
                n.Property(p => p.Valor).HasColumnName("Name").IsRequired().HasMaxLength(50);
                n.HasData(new { UserId = adminId, Valor = "Admin Sistema" });
            });

            builder.OwnsOne(u => u.Email, e =>
            {
                e.Property(p => p.Valor).HasColumnName("Email").IsRequired().HasMaxLength(100);
                e.HasData(new { UserId = adminId, Valor = "admin@fiapcloudgames.com.br" });
            });

            builder.OwnsOne(u => u.Password, s =>
            {
                s.Property(p => p.Hash).HasColumnName("Password").IsRequired().HasMaxLength(60);
                s.HasData(new { UserId = adminId, Hash = "$2a$11$Soy4TsNUDtuazT6CJulPleFnp82cF5BkICiOmF9sk19x0X6pMAic." });
            });

            builder.Property(u => u.DeactivationReason)
                .IsRequired(false);
        }
    }
}
