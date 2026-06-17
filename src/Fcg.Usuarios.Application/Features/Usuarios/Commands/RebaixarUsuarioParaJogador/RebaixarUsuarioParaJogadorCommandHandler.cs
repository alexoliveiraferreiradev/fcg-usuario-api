using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.RebaixarUsuarioParaJogador
{
    public class RebaixarUsuarioParaJogadorCommandHandler : IRequestHandler<RebaixarUsuarioParaJogadorCommand, UsuarioResponse>
    {
        public Task<UsuarioResponse> Handle(RebaixarUsuarioParaJogadorCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
