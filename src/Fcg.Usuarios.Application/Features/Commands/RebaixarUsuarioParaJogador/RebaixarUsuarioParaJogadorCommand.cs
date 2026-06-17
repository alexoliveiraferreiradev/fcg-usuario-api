using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.RebaixarUsuarioParaJogador
{
    public class RebaixarUsuarioParaJogadorCommand : IRequest<UsuarioResponse>
    {
        public Guid Id { get; set; }
        public Guid IdOperador { get; set; }
    }
}
