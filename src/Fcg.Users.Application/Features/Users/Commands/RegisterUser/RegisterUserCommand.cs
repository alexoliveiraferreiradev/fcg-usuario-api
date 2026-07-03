using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Users.Commands.RegisterUser
{
    public record RegisterUserCommand(
        string Name,
        string Email,
        string Password,
        string ConfirmPassword
    ) : IRequest<Guid>;
}
