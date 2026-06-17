using Fcg.Usuarios.Application.Features.Usuarios;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.PromoverUsuarioParaAdmin
{
    public record PromoverUsuarioParaAdminCommand(Guid Id) : IRequest<UsuarioResponse>;
}
