using Dapper;
using Fcg.Users.Domain.Entitites;
using Fcg.Users.Domain.Repositories.Interfaces;
using Fcg.Users.Infrastructure.Persistance;
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
        public void Add(User User)
        {
            _dbContext.Users.Add(User);   
        }

        public void Update(User User)
        {
             _dbContext.Users.Update(User);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var connection = _dbContext.Database.GetDbConnection();
            const string sql = @"SELECT 
                                Id, 
                                Name as Name,
                                Email as Email,
                                Password,
                                Role,
                                IsActive,
                                CreatedAt,
                                UpdatedAt,DeactivationReason
                                from Users where Email = @Email";


            return await connection.QueryFirstOrDefaultAsync<User>(sql,new {Email =  email});  
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        }
            

        public async Task<(bool EmailUsado, bool NomeUsado)> CheckAvailabilityAsync(string email, string Name)
        {
            var connection = _dbContext.Database.GetDbConnection();

            var query = @" SELECT 
            CAST(CASE WHEN EXISTS (SELECT 1 FROM Users WHERE Email = @Email) THEN 1 ELSE 0 END AS BIT) AS EmailUsado,
            CAST(CASE WHEN EXISTS (SELECT 1 FROM Users WHERE Name = @Name) THEN 1 ELSE 0 END AS BIT) AS NomeUsado";
                        
            return await connection.QueryFirstOrDefaultAsync<(bool EmailUsado, bool NomeUsado)>(query, new { Email = email, Name = Name });
        }

        public async Task<bool> HasMultipleAdminsAsync()
        {
            var connection = _dbContext.Database.GetDbConnection();

            var query = @"SELECT CAST(CASE WHEN (SELECT COUNT(1) FROM Users WHERE Role = 1) > 1 THEN 1 ELSE 0 END AS BIT)";

            return await connection.QueryFirstOrDefaultAsync<bool>(query);
        }

       
        public async Task<bool> CheckNameInUseAsync(Guid UserId, string nomeCadastrado)
        {
            var connection = _dbContext.Database.GetDbConnection();

            var query = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM Users WHERE Name = @Name and Id!= @IdUser) THEN 1 ELSE 0 END AS BIT) AS NomeUsado";

            return await connection.QueryFirstOrDefaultAsync<bool>(query, new { Name = nomeCadastrado, IdUser = UserId }); 
        }

        
    }
}
