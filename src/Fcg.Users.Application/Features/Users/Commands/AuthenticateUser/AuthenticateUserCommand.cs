using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Users.Commands.AuthenticateUser
{
    public record AuthenticateUserCommand(
        string Email,
        string Password
    ) : IRequest<LoginResponse>;
}
