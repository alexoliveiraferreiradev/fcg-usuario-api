using MediatR;
using System.ComponentModel;

namespace Fcg.Users.Application.Features.Admin.Commands.ReactiveAccount
{
    /// <summary>
    /// Comando enviado por um administrador para reativar uma conta de usuário desativada.
    /// </summary>
    /// <param name="UserId">Identificador único (GUID) do usuário que terá sua conta reativada.</param>
    public record ReactivateAccountCommand(
        [property: DefaultValue("00000000-0000-0000-0000-000000000000")] Guid UserId
    ) : IRequest;
}
