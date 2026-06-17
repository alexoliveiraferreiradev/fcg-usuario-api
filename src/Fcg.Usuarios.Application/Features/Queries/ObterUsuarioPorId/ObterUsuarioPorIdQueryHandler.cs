using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Queries.ObterUsuarioPorId
{
    public class ObterUsuarioPorIdQueryHandler : IRequestHandler<ObterUsuarioPorIdQuery, UsuarioResponse>
    {
        public Task<UsuarioResponse> Handle(ObterUsuarioPorIdQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
