using Fcg.Users.Domain.Enum;
using MediatR;
using System.ComponentModel;

namespace Fcg.Users.Application.Features.Admin.Commands.DeactiveUser
{
    /// <summary>
    /// Comando enviado por um administrador para desativar a conta de um usuário.
    /// </summary>
    /// <param name="Id">Identificador único (GUID) do usuário a ser desativado.</param>
    /// <param name="IdOperador">Identificador único (GUID) do administrador operador realizando a ação.</param>
    /// <param name="ReasonDeactivation">Motivo detalhado para a desativação da conta do usuário.</param>
    public record DeactivateUserCommand(
        [property: DefaultValue("00000000-0000-0000-0000-000000000000")] Guid Id,
        [property: DefaultValue("00000000-0000-0000-0000-000000000000")] Guid IdOperador,
        [property: DefaultValue(DeactivationReason.Other)] DeactivationReason ReasonDeactivation
    ) : IRequest;
}
