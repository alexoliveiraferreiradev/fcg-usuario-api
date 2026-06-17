using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.AutenticarUsuario
{
    public class AutenticarUsuarioCommand : IRequest<LoginResponse>
    {        
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
