using MediatR;

namespace Fcg.Users.Application.Features.Users.Commands.DesativarConta
{
    public record DesativarContaCommand(Guid Id) : IRequest;
}
