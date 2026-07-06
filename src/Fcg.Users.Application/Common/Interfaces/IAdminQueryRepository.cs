using Fcg.Users.Application.Features.Users.Responses;

namespace Fcg.Users.Application.Common.Interfaces
{
    public interface IAdminQueryRepository
    {
        Task<IEnumerable<UserResponse>> GetAllUsersAsync(CancellationToken cancellation);
        Task<UserResponse?> GetUserByIdAsync(Guid userId, CancellationToken cancellation);
    }
}
