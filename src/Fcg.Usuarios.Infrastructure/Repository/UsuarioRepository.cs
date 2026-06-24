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
            return await _dbContext.Usuarios.AsNoTracking().FirstOrDefaultAsync(p => p.EmailUsuario.Valor.ToLower() == email.ToLower());
        }

        public async Task<Usuario?> ObterPorId(Guid id)
        {
            return await _dbContext.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
        }
            

        public async Task<bool> VerificaEmailCadastrado(string emailCadastrado)
        {
            return await _dbContext.Usuarios.AnyAsync(x => x.EmailUsuario.Valor.ToLower() == emailCadastrado.ToLower());
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
            return await _dbContext.Usuarios
                       .AnyAsync(x => x.NomeUsuario.Valor.ToLower() == nomeCadastrado.ToLower()
                        && x.Id != usuarioId);
        }
    }
}
