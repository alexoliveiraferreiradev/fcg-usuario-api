using Fcg.Usuarios.Application.Features.Usuarios;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Queries.ObterUsuarioPorId
{
    public record ObterUsuarioPorIdQuery : IRequest<UsuarioResponse>;
}
