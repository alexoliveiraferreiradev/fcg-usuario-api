using Dapper;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Enum;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Users.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _dbContext;
        public UserRepository(UserDbContext dbContext)
        {
            _dbContext = dbContext; 
        }
        public void Add(User user)
        {
            _dbContext.Users.Add(user);   
        }

        public void Update(User user)
        {
             _dbContext.Users.Update(user);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {            
            return await _dbContext.Users.Where(x=>x.Email.Valor == email).FirstOrDefaultAsync();
        }
        

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        }
            

        public async Task<(bool EmailUsado, bool NomeUsado)> CheckAvailabilityAsync(string email, string name)
        {
            return await _dbContext.Users.AsNoTracking()
                .Where(u => u.Email.Valor == email || u.Name.Valor == name)
                .Select(u => new { EmailUsado = u.Email.Valor == email, NomeUsado = u.Name.Valor == name })
                .FirstOrDefaultAsync() is { } result
                ? (result.EmailUsado, result.NomeUsado)
                : (false, false);
        }

        public async Task<bool> HasMultipleAdminsAsync()
        {
            return await _dbContext.Users.AsNoTracking()
            .CountAsync(x => x.Role == UserRole.Admin && x.IsActive) > 1;
        }

       
        public async Task<bool> CheckNameInUseAsync(Guid userId, string nomeCadastrado)
        {
            return await _dbContext.Users.AsNoTracking().AnyAsync(x => x.Name.Valor.ToLower() == nomeCadastrado.ToLower() && x.Id != userId);
        }

        
    }
}
