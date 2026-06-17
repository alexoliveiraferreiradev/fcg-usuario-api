using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.DesativarConta
{
    public record DesativarContaCommand(Guid Id) : IRequest;
}
