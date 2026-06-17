using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.PromoverUsuarioParaAdmin
{
    public class PromoverUsuarioParaAdminCommandHandler : IRequestHandler<PromoverUsuarioParaAdminCommand, UsuarioResponse>
    {
        public Task<UsuarioResponse> Handle(PromoverUsuarioParaAdminCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
