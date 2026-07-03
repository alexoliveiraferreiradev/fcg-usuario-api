using MediatR;

namespace Fcg.Users.Application.Features.Users.Commands.DeactivateAccount
{
    public record DesativarContaCommand(Guid Id) : IRequest;
}
