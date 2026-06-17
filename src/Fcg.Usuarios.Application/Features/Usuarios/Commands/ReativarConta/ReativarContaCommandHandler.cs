using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.ReativarConta
{
    public class ReativarContaCommandHandler : IRequestHandler<ReativarContaCommand>
    {
        public Task Handle(ReativarContaCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
