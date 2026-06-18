using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Usuarios.Queries.ObterUsuarioPorId
{
    public record ObterUsuarioPorIdQuery(Guid Id) : IRequest<UsuarioResponse?>;
}
