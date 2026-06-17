using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.PromoverUsuarioParaAdmin
{
    public record PromoverUsuarioParaAdminCommand(Guid Id) : IRequest<UsuarioResponse>;
}
