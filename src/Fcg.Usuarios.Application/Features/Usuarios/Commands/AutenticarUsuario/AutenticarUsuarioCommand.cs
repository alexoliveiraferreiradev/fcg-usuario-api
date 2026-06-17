using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.AutenticarUsuario
{
    public record AutenticarUsuarioCommand(
        string Email,
        string Senha
    ) : IRequest<LoginResponse>;
}
