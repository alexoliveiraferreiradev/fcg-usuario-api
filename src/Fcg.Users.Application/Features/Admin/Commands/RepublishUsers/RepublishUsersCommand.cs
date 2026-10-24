using MediatR;

namespace Fcg.Users.Application.Features.Admin.Commands.RepublishUsers
{
    /// <summary>
    /// Republica todos os usuários no message broker. Retorna a quantidade de usuários republicados.
    /// </summary>
    public record RepublishUsersCommand() : IRequest<int>;
}
