using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.CadastrarUsuario
{
    public class CadastrarUsuarioCommandHandler : IRequestHandler<CadastrarUsuarioCommand, LoginResponse>
    {
        public Task<LoginResponse> Handle(CadastrarUsuarioCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
