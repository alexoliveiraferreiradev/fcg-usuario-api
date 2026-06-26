using Fcg.Usuarios.Application.Features.Usuarios.Commands.DesativarConta;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fcg.Usuario.API.Endpoint
{
    public static class DesativarContaEndpoint
    {
        public static void MapDesativarContaEndpoints(this IEndpointRouteBuilder app)
        {
           var group = app.MapGroup("api/minha-conta").WithTags("Minha Conta").RequireAuthorization();

            group.MapPut("desativar-conta", async (
                [FromBody] DesativarContaCommand command,
                [FromServices] ISender mediator,
                CancellationToken cancellationToken,
                ClaimsPrincipal user) =>
            {
                var currentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(currentId))
                {
                    return Results.Unauthorized();
                }

                var usuarioId = Guid.Parse(currentId);

                var desativarContaCommand = command with { Id = usuarioId };

                await mediator.Send(desativarContaCommand);

                return Results.NoContent();

            })
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status204NoContent);
        }
    }
}
