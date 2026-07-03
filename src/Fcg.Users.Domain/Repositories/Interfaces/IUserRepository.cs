using Fcg.Users.Domain.Entitites;

namespace Fcg.Users.Domain.Repositories.Interfaces
{
    public interface IUserRepository 
    {
        void Add(User User);
        void Update(User User);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);        
        Task<bool> HasMultipleAdminsAsync();
        Task<(bool EmailUsado, bool NomeUsado)> CheckAvailabilityAsync(string email, string Name);        
        Task<bool> CheckNameInUseAsync(Guid UserId, string nomeCadastrado);
    }
}
