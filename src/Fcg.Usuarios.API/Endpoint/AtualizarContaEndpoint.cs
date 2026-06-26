using Fcg.Usuarios.Application.Features.Usuarios.Commands.AtualizarUsuario;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fcg.Usuario.API.Endpoint
{
    public static class AtualizarContaEndpoint
    {
        public static void MapAtualizaContaEndpoints(this IEndpointRouteBuilder app)
        {            
            var group = app.MapGroup("api/minha-conta").WithTags("Minha Conta").RequireAuthorization();

            group.MapPut("atualizar", async (
                [FromBody] AtualizarUsuarioCommand command,
                [FromServices] ISender mediator,
                CancellationToken cancellationToken,
                ClaimsPrincipal user) =>
            {
                var currentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(currentId))
                {
                    return  Results.Unauthorized();
                }

                var usuarioId = Guid.Parse(currentId);

                var atualizarCommand = command with { UsuarioId = usuarioId };

                var response = await mediator.Send(atualizarCommand, cancellationToken);

                if (response == null)
                    return Results.NotFound(new { Mensagem = "Usuário não encontrado." });

                return Results.Ok(response);
            })
            .Produces<UsuarioResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}
