using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.RebaixarUsuarioParaJogador
{
    public record RebaixarUsuarioParaJogadorCommand(
        Guid Id,
        Guid IdOperador
    ) : IRequest<UsuarioResponse>;
}
