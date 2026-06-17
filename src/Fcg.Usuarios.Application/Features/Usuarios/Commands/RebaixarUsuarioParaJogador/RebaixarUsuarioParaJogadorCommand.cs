using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.RebaixarUsuarioParaJogador
{
    public record RebaixarUsuarioParaJogadorCommand(
        Guid Id,
        Guid IdOperador
    ) : IRequest<UsuarioResponse>;
}
