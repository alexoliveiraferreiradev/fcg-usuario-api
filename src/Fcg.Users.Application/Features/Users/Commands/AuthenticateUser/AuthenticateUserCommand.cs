using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Users.Commands.AuthenticateUser
{
    /// <summary>
    /// Comando enviado para autenticar um usuário no sistema.
    /// </summary>
    /// <param name="Email">Endereço de e-mail de acesso cadastrado.</param>
    /// <param name="Password">Senha correspondente cadastrada.</param>
    public record AuthenticateUserCommand(
        string Email,
        string Password
    ) : IRequest<LoginResponse>;
}
