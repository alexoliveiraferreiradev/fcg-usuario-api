using Fcg.Users.Application.Features.Users.Responses;
using MediatR;
using System.ComponentModel;

namespace Fcg.Users.Application.Features.Users.Commands.AuthenticateUser
{
    /// <summary>
    /// Comando enviado para autenticar um usuário no sistema.
    /// </summary>
    /// <param name="Email">Endereço de e-mail de acesso cadastrado.</param>
    /// <param name="Password">Senha correspondente cadastrada.</param>
    public record AuthenticateUserCommand(
        [property: DefaultValue("Email do usuário")] string Email,
        [property: DefaultValue("Senha do usuário")] string Password
    ) : IRequest<LoginResponse>;
}
