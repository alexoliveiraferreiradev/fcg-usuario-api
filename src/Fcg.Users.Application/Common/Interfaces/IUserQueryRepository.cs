using Fcg.Users.Application.Features.Users.Responses;

namespace Fcg.Users.Application.Common.Interfaces
{
    public interface IUserQueryRepository
    {
        Task<UserAccountResponse?> GetUserByIdAsync(Guid userId, CancellationToken cancellation);
    }
}
