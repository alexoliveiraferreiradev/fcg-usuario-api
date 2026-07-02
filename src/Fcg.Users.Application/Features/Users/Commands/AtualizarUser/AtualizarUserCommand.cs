using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Users.Commands.AtualizarUser
{
    public record AtualizarUserCommand(
        Guid UserId,
        string NomeUser,
        string SenhaUser,
        string ConfirmacaoSenha
    ) : IRequest<UserResponse>;
}
