using Fcg.Users.Application.Features.Users.Responses;
using MediatR;
using System.ComponentModel;

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
        [property: DefaultValue("nome do usuário")] string Name,
        [property: DefaultValue("email do usuário")] string Email,
        [property: DefaultValue("senha do usuário")] string Password,
        [property: DefaultValue("confirmação da senha do usuário")] string ConfirmPassword
    ) : IRequest<Guid>;
}
