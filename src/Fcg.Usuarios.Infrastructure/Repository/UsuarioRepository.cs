using Dapper;
using Fcg.Usuarios.Domain.Entitites;
using Fcg.Usuarios.Domain.Enum;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using Fcg.Usuarios.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Usuarios.Infrastructure.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UsuarioDbContext _dbContext;
        public UsuarioRepository(UsuarioDbContext dbContext)
        {
            _dbContext = dbContext; 
        }
        public void Adicionar(Usuario usuario)
        {
            _dbContext.Usuarios.Add(usuario);   
        }

        public void Atualizar(Usuario usuario)
        {
             _dbContext.Usuarios.Update(usuario);
        }

        public async Task<Usuario?> ObterPorEmail(string email)
        {
            var connection = _dbContext.Database.GetDbConnection();
            var query = @"Select 
                        Id,Nome,Email,Senha,Perfil,Ativo, 
                        DataCadastro,DataAlteracao,MotivoDesativacao 
                        from Usuarios where Email = @Email";
            
            return await connection.QueryFirstAsync<Usuario?>(query, new {Email = email});
        }

        public async Task<Usuario?> ObterPorId(Guid id)
        {
            return await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
        }
            

        public async Task<bool> VerificaEmailCadastrado(string emailCadastrado)
        {
            return await _dbContext.Usuarios.AnyAsync(x => x.EmailUsuario.Valor.ToLower() == emailCadastrado.ToLower());
        }

        public async Task<(bool EmailUsado, bool NomeUsado)> VerificaIndisponibilidade(string email, string nome)
        {
            var connection = _dbContext.Database.GetDbConnection();

            var query = @" SELECT 
            CAST(CASE WHEN EXISTS (SELECT 1 FROM Usuarios WHERE Email = @Email) THEN 1 ELSE 0 END AS BIT) AS EmailUsado,
            CAST(CASE WHEN EXISTS (SELECT 1 FROM Usuarios WHERE Nome = @Nome) THEN 1 ELSE 0 END AS BIT) AS NomeUsado";
                        
            return await connection.QueryFirstOrDefaultAsync<(bool EmailUsado, bool NomeUsado)>(query, new { Email = email, Nome = nome });
        }

        public async Task<bool> VerificaMaisDeUmAdminCadastrado()
        {
            return await _dbContext.Usuarios
        .CountAsync(x => x.Perfil == TipoUsuario.Administrador && x.Ativo) > 1;
        }

        public async Task<bool> VerificaNomeCadastrado(string nomeCadastrado)
        {
            return await _dbContext.Usuarios.AnyAsync(x => x.NomeUsuario.Valor.ToLower() == nomeCadastrado.ToLower());
        }

        public async Task<bool> VerificaNomeCadastradoParaAlteracao(Guid usuarioId, string nomeCadastrado)
        {
            var connection = _dbContext.Database.GetDbConnection();

            var query = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM Usuarios WHERE Nome = @Nome and Id!= @IdUsuario) THEN 1 ELSE 0 END AS BIT) AS NomeUsado";

            return await connection.QueryFirstOrDefaultAsync<bool>(query, new { Nome = nomeCadastrado, IdUsuario = usuarioId }); 
        }

        
    }
}
