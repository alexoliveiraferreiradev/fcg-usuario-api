using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Queries.ObterUsuarioPorEmail
{
    public record ObterUsuarioPorEmailQuery : IRequest<UsuarioResponse>;
}
