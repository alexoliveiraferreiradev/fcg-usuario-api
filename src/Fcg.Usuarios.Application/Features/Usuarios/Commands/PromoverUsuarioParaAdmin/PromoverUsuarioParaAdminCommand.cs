using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.PromoverUsuarioParaAdmin
{
    public record PromoverUsuarioParaAdminCommand(Guid Id,Guid IdOperador) : IRequest<UsuarioResponse>;
}
