using Fcg.Usuarios.Application.Features.Usuarios.Commands.PromoverUsuarioParaAdmin;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fcg.Usuario.API.Endpoint
{
    public static class PromoverUsuarioEndpoint
    {
        public static void MapPromoverContaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/usuarios").WithTags("Gerenciamento de Usuários").RequireAuthorization();

            group.MapPut("promover-para-admin/{id:guid}", async (
                ClaimsPrincipal user,
                [FromRoute] Guid id,
                [FromServices] ISender mediator,
                CancellationToken cancellationToken) =>
            {
                var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(currentUserIdClaim))
                    return Results.Unauthorized();

                var adminId = Guid.Parse(currentUserIdClaim);

                var usuarioAPromover = new PromoverUsuarioParaAdminCommand(id, adminId);

                var response = await mediator.Send(usuarioAPromover);

                return Results.NoContent();

            })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
