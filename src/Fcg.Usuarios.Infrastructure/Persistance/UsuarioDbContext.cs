using Fcg.Usuarios.Domain.Entitites;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Usuarios.Infrastructure.Persistance
{
    public class UsuarioDbContext : DbContext
    {
        public UsuarioDbContext(DbContextOptions<UsuarioDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsuarioDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
