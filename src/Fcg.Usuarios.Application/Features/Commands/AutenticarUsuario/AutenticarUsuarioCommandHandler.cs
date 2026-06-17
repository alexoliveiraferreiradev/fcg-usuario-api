using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.AutenticarUsuario
{
    public class AutenticarUsuarioCommandHandler : IRequestHandler<AutenticarUsuarioCommand, LoginResponse>
    {
        public Task<LoginResponse> Handle(AutenticarUsuarioCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
