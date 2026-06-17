using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.PromoverUsuarioParaAdmin
{
    public record PromoverUsuarioParaAdminCommand(Guid Id) : IRequest<UsuarioResponse>;
}
