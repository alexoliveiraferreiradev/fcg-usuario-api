using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Users.Commands.CadastrarUser
{
    public record CadastrarUserCommand(
        string Nome,
        string Email,
        string Senha,
        string ConfirmacaoSenha
    ) : IRequest<Guid>;
}
