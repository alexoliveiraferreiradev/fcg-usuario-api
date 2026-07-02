using Fcg.Users.Domain.Enum;
using MediatR;

namespace Fcg.Users.Application.Features.Admin.Commands.DesativarUser
{
    public record DesativarUserCommand(
        Guid Id,
        Guid IdOperador,
        DeactivationReason MotivoDelecao
    ) : IRequest;
}
