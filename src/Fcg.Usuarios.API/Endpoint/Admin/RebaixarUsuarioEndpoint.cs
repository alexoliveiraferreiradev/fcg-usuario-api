using Fcg.Usuarios.Application.Features.Admin.Commands.RebaixarUsuarioParaJogador;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fcg.Usuario.API.Endpoint.Admin
{
    public static class RebaixarUsuarioEndpoint
    {
        public static void MapRabaixarContaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/usuarios").WithTags("Gerenciamento de Usuários").RequireAuthorization();

            group.MapPut("rebaixar-jogador/{id:guid}", async (
                [FromRoute] Guid id,
                [FromServices] ISender mediator,
                CancellationToken cancellationToken,
                ClaimsPrincipal user) =>
            {
                var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var currentUserId = Guid.Parse(currentUserIdClaim);

                if (id == currentUserId)
                {
                    return Results.BadRequest("Você não possui permissão para rebaixar a própria conta enquanto logado");
                }

                var rebaixarJogadorCommand = new RebaixarUsuarioParaJogadorCommand(id, currentUserId);

                await mediator.Send(rebaixarJogadorCommand);

                return Results.NoContent();
            })
             .Produces(StatusCodes.Status204NoContent)
             .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
