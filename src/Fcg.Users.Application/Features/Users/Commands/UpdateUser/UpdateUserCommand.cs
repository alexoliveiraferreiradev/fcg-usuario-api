using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Users.Commands.UpdateUser
{
    public record UpdateUserCommand(
        Guid UserId,
        string Name,
        string password,
        string ConfirmacaoSenha
    ) : IRequest<UserResponse>;
}
