using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Users.Commands.CadastrarUser
{
    public record RegisterUserCommand(
        string Name,
        string Email,
        string Password,
        string ConfirmacaoSenha
    ) : IRequest<Guid>;
}
