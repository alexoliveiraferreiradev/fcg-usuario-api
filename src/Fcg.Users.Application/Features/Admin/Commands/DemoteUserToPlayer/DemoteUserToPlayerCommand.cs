using Fcg.Users.Application.Features.Users.Responses;
using MediatR;
using System.ComponentModel;

namespace Fcg.Users.Application.Features.Admin.Commands.DemoteUserToPlayer
{
    /// <summary>
    /// Comando enviado por um administrador para rebaixar o perfil de um administrador para jogador comum.
    /// </summary>
    /// <param name="Id">Identificador único (GUID) do usuário que terá seu perfil rebaixado.</param>
    /// <param name="IdOperador">Identificador único (GUID) do administrador operador realizando a ação.</param>
    public record DemoteUserToPlayerCommand(
        [property: DefaultValue("00000000-0000-0000-0000-000000000000")] Guid Id,
        [property: DefaultValue("00000000-0000-0000-0000-000000000000")] Guid IdOperador
    ) : IRequest<UserResponse>;
}
