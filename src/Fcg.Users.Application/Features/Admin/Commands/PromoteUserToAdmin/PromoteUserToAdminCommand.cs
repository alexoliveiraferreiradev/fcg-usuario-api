using Fcg.Users.Application.Features.Users.Responses;
using MediatR;
using System.ComponentModel;

namespace Fcg.Users.Application.Features.Admin.Commands.PromoteUserToAdmin
{
    /// <summary>
    /// Comando enviado por um administrador para promover um usuário para a função de Administrador.
    /// </summary>
    /// <param name="Id">Identificador único (GUID) do usuário a ser promovido.</param>
    /// <param name="IdOperador">Identificador único (GUID) do administrador operador realizando a ação.</param>
    public record PromoteUserToAdminCommand(
        [property: DefaultValue("00000000-0000-0000-0000-000000000000")] Guid Id,
        [property: DefaultValue("00000000-0000-0000-0000-000000000000")] Guid IdOperador
    ) : IRequest<UserResponse>;
}
