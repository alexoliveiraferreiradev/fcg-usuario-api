using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Users.Commands.AutenticarUser
{
    public record AutenticarUserCommand(
        string Email,
        string Senha
    ) : IRequest<LoginResponse>;
}
