using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.DesativarConta
{
    public record DesativarContaCommand(Guid Id) : IRequest;
}
