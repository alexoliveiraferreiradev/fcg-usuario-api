using Fcg.Usuarios.Application.Features.Usuarios;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Queries.ObterTodosUsuarios
{
    public record ObterTodosUsuariosQuery : IRequest<IEnumerable<UsuarioResponse>>;
}
