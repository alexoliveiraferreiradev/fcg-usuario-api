using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.AtualizarUsuario
{
    public class AtualizarUsuarioCommandHandler : IRequestHandler<AtualizarUsuarioCommand, UsuarioResponse>
    {
        public Task<UsuarioResponse> Handle(AtualizarUsuarioCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
