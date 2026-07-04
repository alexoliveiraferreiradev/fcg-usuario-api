using Fcg.Core.Abstractions.Interfaces;

namespace Fcg.Users.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserDbContext _dbContext;
        public UnitOfWork(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CommitAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
