using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Admin.Commands.PromoverUserParaAdmin
{
    public record PromoteUserToAdminCommand(Guid Id,Guid IdOperador) : IRequest<UserResponse>;
}
