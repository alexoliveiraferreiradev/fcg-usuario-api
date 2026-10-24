using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Users.Queries.GetMyAccount
{
    /// <summary>
    /// Dados da conta do próprio usuário autenticado. O UserId vem das claims do token, nunca da requisição.
    /// </summary>
    public record GetMyAccountQuery(Guid UserId) : IRequest<UserAccountResponse?>;
}
