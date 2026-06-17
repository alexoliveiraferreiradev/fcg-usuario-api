using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.DesativarConta
{
    public class DesativarContaCommandHandler : IRequestHandler<DesativarContaCommand>
    {
        public Task Handle(DesativarContaCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
