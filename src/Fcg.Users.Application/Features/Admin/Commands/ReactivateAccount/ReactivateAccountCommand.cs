using MediatR;

namespace Fcg.Users.Application.Features.Admin.Commands.ReativarConta
{
    public record ReactivateAccountCommand(Guid UserId) : IRequest;
}
