using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Admin.Queries.ObterTodosUsers
{
    public record GetAllUsersQuery : IRequest<IEnumerable<UserResponse>>;
}
