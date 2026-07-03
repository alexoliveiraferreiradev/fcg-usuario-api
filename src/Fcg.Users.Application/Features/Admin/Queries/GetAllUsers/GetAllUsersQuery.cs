using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Admin.Queries.GetAllUsers
{
    public record GetAllUsersQuery : IRequest<IEnumerable<UserResponse>>;
}
