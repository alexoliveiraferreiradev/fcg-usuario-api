using Fcg.Usuarios.Application.Features.Admin.Queries.ObterUsuarioPorId;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Usuario.API.Queries
{
    public static class UsuarioQueryEndpoint
    {
        public static void MapObtemUsuarioEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/usuarios").RequireAuthorization().WithTags("Gerencimanto de Usuários");

            group.MapGet("obtem-usuario/{id:guid}", async (
                [FromRoute] Guid id,
                [FromServices] ISender mediator,
                CancellationToken CancellationToken) =>
            {
                var query = new ObterUsuarioPorIdQuery(id);
                var usuario = await mediator.Send(query, CancellationToken);

                if (usuario == null )
                    return Results.Ok(Enumerable.Empty<UsuarioResponse>);

                return Results.Ok(usuario);
            })
            .Produces<IEnumerable<UsuarioResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized); ;
        }
    }
}
