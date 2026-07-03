using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Admin.Queries.GetUserById
{
    public record GetUserByIdQuery(Guid Id) : IRequest<UserResponse?>;
}
