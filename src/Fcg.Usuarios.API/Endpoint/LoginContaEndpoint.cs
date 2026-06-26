using Fcg.Usuarios.Application.Features.Usuarios.Commands.AutenticarUsuario;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Usuario.API.Endpoint
{
    public static class LoginContaEndpoint
    {
        public static void MapLoginEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/login").AllowAnonymous().WithTags("Login");

            group.MapPost("login", async (
                [FromBody] AutenticarUsuarioCommand command,
                [FromServices] ISender mediator,
                CancellationToken cancellationToken) =>
            {
                var response = await mediator.Send(command, cancellationToken);

                if(response == null)
                    return Results.NotFound(new { Mensagem = "Usuário não encontrado." });

                return Results.Ok(response);
            })
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}
