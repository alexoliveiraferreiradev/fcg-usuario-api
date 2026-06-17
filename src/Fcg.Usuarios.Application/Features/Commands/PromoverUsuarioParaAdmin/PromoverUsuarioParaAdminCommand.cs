using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.PromoverUsuarioParaAdmin
{
    public class PromoverUsuarioParaAdminCommand : IRequest<UsuarioResponse>
    {
        public Guid Id { get; set; }
    }
}
