using MediatR;
using System.ComponentModel;

namespace Fcg.Users.Application.Features.Users.Commands.DeactivateAccount
{
    /// <summary>
    /// Comando enviado pelo próprio usuário autenticado para solicitar a desativação da sua conta.
    /// </summary>
    /// <param name="Id">Identificador único (GUID) da conta do usuário logado.</param>
    public record DeactiveAccountCommand(
        [property: DefaultValue("00000000-0000-0000-0000-000000000000")] Guid Id
    ) : IRequest;
}
