using MediatR;

namespace Fcg.Usuarios.Application.Features.Admin.Commands.ReativarConta
{
    public record ReativarContaCommand(Guid UsuarioId) : IRequest;
}
