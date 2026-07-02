using Fcg.Users.Domain.Entitites;

namespace Fcg.Users.Domain.Repositories.Interfaces
{
    public interface IUserRepository 
    {
        void Adicionar(User User);
        void Atualizar(User User);
        Task<User?> ObterPorId(Guid id);
        Task<User?> ObterPorEmail(string email);        
        Task<bool> VerificaMaisDeUmAdminCadastrado();
        Task<(bool EmailUsado, bool NomeUsado)> VerificaIndisponibilidade(string email, string nome);        
        Task<bool> VerificaNomeCadastradoParaAlteracao(Guid UserId, string nomeCadastrado);
    }
}
