using Fcg.Usuarios.Domain.Enum;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Admin.Commands.DesativarUsuario
{
    public record DesativarUsuarioCommand(
        Guid Id,
        Guid IdOperador,
        MotivoDesativacao MotivoDelecao
    ) : IRequest;
}
