using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.CadastrarUsuario
{
    public record CadastrarUsuarioCommand(
        string Nome,
        string Email,
        string Senha,
        string ConfirmacaoSenha
    ) : IRequest<LoginResponse>;
}
