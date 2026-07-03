using MediatR;

namespace Fcg.Users.Application.Features.Admin.Commands.ReactiveAccount
{
    public record ReactivateAccountCommand(Guid UserId) : IRequest;
}
