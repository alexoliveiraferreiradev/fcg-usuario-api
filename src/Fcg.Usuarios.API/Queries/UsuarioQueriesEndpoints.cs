using Fcg.Usuarios.Application.Features.Usuarios.Queries.ObterTodosUsuarios;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Usuario.API.Queries
{
    public static class UsuarioQueriesEndpoints
    {
        public static void MapListaUsuarioEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/usuarios").RequireAuthorization().WithTags("Gerencimanto de Usuários");

            group.MapGet("obtem-todos", async (
                [FromQuery] int? pagina,
                [FromQuery] int? limite,
                [FromServices] ISender mediator,
                CancellationToken CancellationToken) =>
            {
                var query = new ObterTodosUsuariosQuery();
                var usuarios = await mediator.Send(query, CancellationToken);

                if (usuarios == null || !usuarios.Any())
                    return Results.Ok(Enumerable.Empty<UsuarioResponse>);

                return Results.Ok(usuarios);
            })
            .Produces<IEnumerable<UsuarioResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized); ;
        }
    }
}
