using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.DesativarConta
{
    public class DesativarContaCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
