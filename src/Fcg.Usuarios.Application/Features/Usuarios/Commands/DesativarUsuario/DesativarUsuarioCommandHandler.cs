using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.DesativarUsuario
{
    public class DesativarUsuarioCommandHandler : IRequestHandler<DesativarUsuarioCommand>
    {
        public Task Handle(DesativarUsuarioCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
