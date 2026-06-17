using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.AutenticarUsuario
{
    public record AutenticarUsuarioCommand(
        string Email,
        string Senha
    ) : IRequest<LoginResponse>;
}
