using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.RebaixarUsuarioParaJogador
{
    public class RebaixarUsuarioParaJogadorCommandHandler : IRequestHandler<RebaixarUsuarioParaJogadorCommand, UsuarioResponse>
    {
        public Task<UsuarioResponse> Handle(RebaixarUsuarioParaJogadorCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
