using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.AtualizarUsuario
{
    public record AtualizarUsuarioCommand(
        Guid UsuarioId,
        string NomeUsuario,
        string SenhaUsuario,
        string ConfirmacaoSenha
    ) : IRequest<UsuarioResponse>;
}
