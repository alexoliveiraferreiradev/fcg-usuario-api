using Fcg.Usuarios.Application.Features.Usuarios;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Queries.ObterUsuarioPorEmail
{
    public record ObterUsuarioPorEmailQuery : IRequest<UsuarioResponse>;
}
