using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Queries.ObterTodosUsuarios
{
    public record ObterTodosUsuariosQuery : IRequest<IEnumerable<UsuarioResponse>>;
}
