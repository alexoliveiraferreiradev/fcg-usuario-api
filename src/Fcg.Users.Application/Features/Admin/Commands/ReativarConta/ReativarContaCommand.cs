using MediatR;

namespace Fcg.Users.Application.Features.Admin.Commands.ReativarConta
{
    public record ReativarContaCommand(Guid UserId) : IRequest;
}
