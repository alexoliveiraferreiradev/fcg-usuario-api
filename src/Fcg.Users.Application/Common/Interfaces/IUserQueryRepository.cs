namespace Fcg.Users.Application.Common.Interfaces
{
    public interface IUserQueryRepository
    {
        Task<(string NomeUsuario, string EmailUsuario)> GetUserInfoByIdAysnc(Guid userId);
    }
}
