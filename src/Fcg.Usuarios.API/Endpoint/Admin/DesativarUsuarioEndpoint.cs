using Fcg.Usuarios.Application.Features.Admin.Commands.DesativarUsuario;
using Fcg.Usuarios.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fcg.Usuario.API.Endpoint.Admin
{
    public static class DesativarUsuarioEndpoint
    {
        public static void MapDesativarUsuarioEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/usuarios").WithTags("Gerenciamento de Usuários").RequireAuthorization();

            group.MapPut("desativar-usuario/{id:guid}", async (
                [FromRoute] Guid id,
                [FromServices] ISender mediator,
                [FromBody] MotivoDesativacao motivoDesativacao,
                ClaimsPrincipal user,
                CancellationToken cancellationToken
                ) =>
            {
                var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserIdClaim))
                {                 
                    return Results.Unauthorized();
                }

                var currentUserId = Guid.Parse(currentUserIdClaim); 

                var desativarUsuarioCommand = new DesativarUsuarioCommand(id, currentUserId, motivoDesativacao);

                await mediator.Send(desativarUsuarioCommand);

                return Results.NoContent(); 
            })
             .Produces(StatusCodes.Status204NoContent)
             .Produces(StatusCodes.Status400BadRequest)
             .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}
