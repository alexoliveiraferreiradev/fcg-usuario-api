using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Admin.Commands.DemoteUserToPlayer
{
    public record DemoteUserToPlayerCommand(
        Guid Id,
        Guid IdOperador
    ) : IRequest<UserResponse>;
}
