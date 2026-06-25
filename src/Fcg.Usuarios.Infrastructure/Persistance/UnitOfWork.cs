using Fcg.Core.Abstractions.Interfaces;

namespace Fcg.Usuarios.Infrastructure.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UsuarioDbContext _dbContext;
        public UnitOfWork(UsuarioDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
