using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Queries.ObterTodosUsuarios
{
    public class ObterTodosUsuariosQueryHandler : IRequestHandler<ObterTodosUsuariosQuery, IEnumerable<UsuarioResponse>>
    {
        public Task<IEnumerable<UsuarioResponse>> Handle(ObterTodosUsuariosQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
