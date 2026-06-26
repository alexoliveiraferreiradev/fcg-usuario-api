using Fcg.Usuarios.Application.Features.Usuarios.Commands.AtualizarUsuario;
using Fcg.Usuarios.Application.Features.Usuarios.Commands.AutenticarUsuario;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Usuario.API.Endpoint.Usuario
{
    public static class AutenticarContaEndpoint
    {
        public static void MapAutenticarEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/login").AllowAnonymous().WithTags("Autenticação do Usuário");

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
            .Produces(StatusCodes.Status404NotFound)
            .AddEndpointFilter<ValidationFilter<AutenticarUsuarioCommand>>();
        }
    }
}
