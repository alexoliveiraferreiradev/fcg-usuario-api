using Fcg.Usuarios.Domain.Enum;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.DesativarUsuario
{
    public class DesativarUsuarioCommand : IRequest
    {
        public Guid Id { get; set; }
        public Guid IdOperador { get; set; }
        public MotivoDesativacao MotivoDelecao { get; set; }
    }
}
