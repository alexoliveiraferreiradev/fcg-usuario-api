using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.CadastrarUsuario
{
    public record CadastrarUsuarioCommand(
        string Nome,
        string Email,
        string Senha,
        string ConfirmacaoSenha
    ) : IRequest<LoginResponse>;
}
