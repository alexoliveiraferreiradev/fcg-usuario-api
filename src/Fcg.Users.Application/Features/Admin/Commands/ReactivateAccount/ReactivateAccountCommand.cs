using MediatR;

namespace Fcg.Users.Application.Features.Admin.Commands.ReactiveAccount
{
    /// <summary>
    /// Comando enviado por um administrador para reativar uma conta de usuário desativada.
    /// </summary>
    /// <param name="UserId">Identificador único (GUID) do usuário que terá sua conta reativada.</param>
    public record ReactivateAccountCommand(Guid UserId) : IRequest;
}
