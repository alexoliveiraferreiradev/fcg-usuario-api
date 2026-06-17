using Fcg.Usuarios.Application.Features.Usuarios;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.RebaixarUsuarioParaJogador
{
    public record RebaixarUsuarioParaJogadorCommand(
        Guid Id,
        Guid IdOperador
    ) : IRequest<UsuarioResponse>;
}
