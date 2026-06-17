using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Queries.ObterUsuarioPorId
{
    public record ObterUsuarioPorIdQuery : IRequest<UsuarioResponse>;
}
