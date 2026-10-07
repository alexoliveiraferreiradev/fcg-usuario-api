using Fcg.Users.Domain.Entitites;

namespace Fcg.Users.Domain.Repositories.Interfaces
{
    public interface IUserRepository 
    {
        void Add(User user);
        void Update(User user);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);        
        Task<bool> HasMultipleAdminsAsync();
        Task<(bool EmailUsado, bool NomeUsado)> CheckAvailabilityAsync(string email, string name);        
        Task<bool> CheckNameInUseAsync(Guid userId, string nomeCadastrado);
    }
}
