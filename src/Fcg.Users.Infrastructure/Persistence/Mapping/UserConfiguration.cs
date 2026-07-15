using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

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

            var adminId = Guid.Parse("96a8470a-4712-4eb2-a169-42b7818eb8bb");

            builder.HasData(new
            {
                Id = adminId,
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = new DateTime(2026, 7, 14, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 7, 14, 0, 0, 0, DateTimeKind.Utc)
            });

            builder.OwnsOne(u => u.Name, n =>
            {
                n.Property(p => p.Value).HasColumnName("Name").IsRequired().HasMaxLength(50);
                n.HasData(new { UserId = adminId, Value = "Admin Sistema" });
            });

            builder.OwnsOne(u => u.Email, e =>
            {
                e.Property(p => p.Value).HasColumnName("Email").IsRequired().HasMaxLength(100);
                e.HasIndex(p => p.Value).IsUnique();
                e.HasData(new { UserId = adminId, Value = "admin@fiapcloudgames.com.br" });
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
