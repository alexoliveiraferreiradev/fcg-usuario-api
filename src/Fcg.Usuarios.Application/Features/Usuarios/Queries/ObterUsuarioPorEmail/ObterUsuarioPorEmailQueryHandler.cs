using Fcg.Usuarios.Application.Features.Usuarios;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Queries.ObterUsuarioPorEmail
{
    public class ObterUsuarioPorEmailQueryHandler : IRequestHandler<ObterUsuarioPorEmailQuery, UsuarioResponse>
    {
        public Task<UsuarioResponse> Handle(ObterUsuarioPorEmailQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
