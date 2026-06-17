using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.ReativarConta
{
    public class ReativarContaCommand : IRequest
    {
        public Guid UsuarioId { get; set; }
    }
}
