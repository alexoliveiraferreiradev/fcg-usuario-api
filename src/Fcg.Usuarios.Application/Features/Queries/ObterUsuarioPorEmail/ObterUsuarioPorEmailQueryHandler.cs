using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Queries.ObterUsuarioPorEmail
{
    public class ObterUsuarioPorEmailQueryHandler : IRequestHandler<ObterUsuarioPorEmailQuery, UsuarioResponse>
    {
        public Task<UsuarioResponse> Handle(ObterUsuarioPorEmailQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
