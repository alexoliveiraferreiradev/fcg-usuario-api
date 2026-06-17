using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.ReativarConta
{
    public record ReativarContaCommand(Guid UsuarioId) : IRequest;
}
