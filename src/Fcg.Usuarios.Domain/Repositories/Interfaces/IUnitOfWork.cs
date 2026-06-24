namespace Fcg.Usuarios.Domain.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync();
    }
}
