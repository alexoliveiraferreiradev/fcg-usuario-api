using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Users.Commands.RegisterUser
{
    /// <summary>
    /// Comando enviado para registrar um novo usuário no sistema.
    /// </summary>
    /// <param name="Name">Nome de exibição completo do usuário.</param>
    /// <param name="Email">Endereço de e-mail do usuário (deve ser único).</param>
    /// <param name="Password">Senha desejada para a conta.</param>
    /// <param name="ConfirmPassword">Confirmação idêntica da senha digitada.</param>
    public record RegisterUserCommand(
        string Name,
        string Email,
        string Password,
        string ConfirmPassword
    ) : IRequest<Guid>;
}
