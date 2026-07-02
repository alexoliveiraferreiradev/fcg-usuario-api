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
        public void Adicionar(User User)
        {
            _dbContext.Users.Add(User);   
        }

        public void Atualizar(User User)
        {
             _dbContext.Users.Update(User);
        }

        public async Task<User?> ObterPorEmail(string email)
        {
            var connection = _dbContext.Database.GetDbConnection();
            const string sql = @"SELECT 
                                Id, 
                                Nome as NomeUser,
                                Email as EmailUser,
                                Senha,
                                Perfil,
                                Ativo,
                                DataCadastro,
                                DataAlteracao,MotivoDesativacao
                                from Users where Email = @Email";


            return await connection.QueryFirstOrDefaultAsync<User>(sql,new {Email =  email});  
        }

        public async Task<User?> ObterPorId(Guid id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        }
            

        public async Task<(bool EmailUsado, bool NomeUsado)> VerificaIndisponibilidade(string email, string nome)
        {
            var connection = _dbContext.Database.GetDbConnection();

            var query = @" SELECT 
            CAST(CASE WHEN EXISTS (SELECT 1 FROM Users WHERE Email = @Email) THEN 1 ELSE 0 END AS BIT) AS EmailUsado,
            CAST(CASE WHEN EXISTS (SELECT 1 FROM Users WHERE Nome = @Nome) THEN 1 ELSE 0 END AS BIT) AS NomeUsado";
                        
            return await connection.QueryFirstOrDefaultAsync<(bool EmailUsado, bool NomeUsado)>(query, new { Email = email, Nome = nome });
        }

        public async Task<bool> VerificaMaisDeUmAdminCadastrado()
        {
            var connection = _dbContext.Database.GetDbConnection();

            var query = @"SELECT CAST(CASE WHEN (SELECT COUNT(1) FROM Users WHERE Perfil = 1) > 1 THEN 1 ELSE 0 END AS BIT)";

            return await connection.QueryFirstOrDefaultAsync<bool>(query);
        }

       
        public async Task<bool> VerificaNomeCadastradoParaAlteracao(Guid UserId, string nomeCadastrado)
        {
            var connection = _dbContext.Database.GetDbConnection();

            var query = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM Users WHERE Nome = @Nome and Id!= @IdUser) THEN 1 ELSE 0 END AS BIT) AS NomeUsado";

            return await connection.QueryFirstOrDefaultAsync<bool>(query, new { Nome = nomeCadastrado, IdUser = UserId }); 
        }

        
    }
}
