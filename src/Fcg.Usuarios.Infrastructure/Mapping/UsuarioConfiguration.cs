using Fcg.Usuarios.Domain.Entitites;
using Fcg.Usuarios.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Usuarios.Infrastructure.Mapping
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public UsuarioConfiguration()
        {
        }
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");
            builder.HasKey(p => p.Id);

            var adminId = Guid.Parse("AEA0B4F3-D220-4C8D-ABA8-D868BE7CA593");
            builder.HasData(new
            {
                Id = adminId,
                Ativo = true,
                Perfil = TipoUsuario.Administrador,
                DataCadastro = new DateTime(2026, 5, 2),
                DataAlteracao = new DateTime(2026, 5, 2)
            });

            builder.OwnsOne(u => u.NomeUsuario, n =>
            {
                n.Property(p => p.Valor).HasColumnName("Nome").IsRequired().HasMaxLength(50);
                n.HasData(new { UsuarioId = adminId, Valor = "Administrador Sistema" });
            });

            builder.OwnsOne(u => u.EmailUsuario, e =>
            {
                e.Property(p => p.Valor).HasColumnName("Email").IsRequired().HasMaxLength(100);
                e.HasData(new { UsuarioId = adminId, Valor = "admin@fiapcloudgames.com.br" });
            });

            builder.OwnsOne(u => u.Senha, s =>
            {
                s.Property(p => p.Hash).HasColumnName("Senha").IsRequired().HasMaxLength(60);
                s.HasData(new { UsuarioId = adminId, Hash = "$2a$11$Soy4TsNUDtuazT6CJulPleFnp82cF5BkICiOmF9sk19x0X6pMAic." });
            });

            builder.Property(u => u.MotivoDesativacao)
                .IsRequired(false);
        }
    }
}
