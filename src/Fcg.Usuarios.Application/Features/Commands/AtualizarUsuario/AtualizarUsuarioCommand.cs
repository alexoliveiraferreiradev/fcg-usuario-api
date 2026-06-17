using Fcg.Usuarios.Application.Features.Usuarios;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.AtualizarUsuario
{
    public record AtualizarUsuarioCommand(
        Guid UsuarioId,
        string NomeUsuario,
        string SenhaUsuario,
        string ConfirmacaoSenha
    ) : IRequest<UsuarioResponse>;
}
