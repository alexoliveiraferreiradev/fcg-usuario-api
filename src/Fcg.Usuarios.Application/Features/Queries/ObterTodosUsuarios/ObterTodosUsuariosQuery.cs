using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Queries.ObterTodosUsuarios
{
    public record ObterTodosUsuariosQuery : IRequest<IEnumerable<UsuarioResponse>>;
}
