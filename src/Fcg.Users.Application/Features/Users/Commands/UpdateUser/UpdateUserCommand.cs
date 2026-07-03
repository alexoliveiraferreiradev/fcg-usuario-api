using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Users.Commands.UpdateUser
{
    /// <summary>
    /// Comando enviado pelo próprio usuário autenticado para atualizar seus dados cadastrais.
    /// </summary>
    /// <param name="UserId">Identificador único (GUID) do usuário logado.</param>
    /// <param name="Name">Novo nome completo a ser atualizado.</param>
    /// <param name="Password">Nova senha a ser cadastrada.</param>
    /// <param name="ConfirmPassword">Confirmação idêntica da nova senha.</param>
    public record UpdateUserCommand(
        Guid UserId,
        string Name,
        string Password,
        string ConfirmPassword
    ) : IRequest<UserResponse>;
}
