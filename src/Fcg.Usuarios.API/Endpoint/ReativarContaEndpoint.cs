using Fcg.Usuarios.Application.Features.Usuarios.Commands.ReativarConta;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fcg.Usuario.API.Endpoint
{
    public static class ReativarContaEndpoint
    {
        public static void MapReativarContaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/usuarios").WithTags("Gerenciamento de Usuários").RequireAuthorization();

            group.MapPut("reativar-jogador/{id:guid}", async (
                [FromRoute] Guid id,
                [FromServices] ISender mediator,
                CancellationToken cancellationToken,
                ClaimsPrincipal user) =>
            {
                var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(currentUserIdClaim))
                {
                    return Results.Unauthorized();
                }
                var reativarJogadorCommand = new ReativarContaCommand(id);

                await mediator.Send(reativarJogadorCommand);

                return Results.NoContent();
            })
             .Produces(StatusCodes.Status204NoContent)
             .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}
