using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.ReativarConta
{
    public record ReativarContaCommand(Guid UsuarioId) : IRequest;
}
