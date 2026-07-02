using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Admin.Queries.ObterUserPorId
{
    public record ObterUserPorIdQuery(Guid Id) : IRequest<UserResponse?>;
}
