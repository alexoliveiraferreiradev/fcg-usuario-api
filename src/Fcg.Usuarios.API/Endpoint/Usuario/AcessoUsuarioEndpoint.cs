using Fcg.Usuarios.Application.Features.Usuarios.Commands.AutenticarUsuario;
using Fcg.Usuarios.Application.Features.Usuarios.Commands.CadastrarUsuario;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Usuario.API.Endpoint.Usuario
{
    public static class AcessoUsuarioEndpoint
    {
        public static void MapAcessoUsuarioEndpoint(this WebApplication app)
        {
            var group = app.MapGroup("/api/usuario").WithTags("Autenticação do Usuário");

            group.MapPost("/login", AutenticarConta)
                .Produces<LoginResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/cadastrar", CadastrarConta)
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
        }

        private static async Task<IResult> AutenticarConta(
            [FromBody] AutenticarUsuarioCommand command,
            [FromServices] ISender mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(command, cancellationToken);

            if (response == null)
                return Results.NotFound(new { Mensagem = "Usuário não encontrado." });

            return Results.Ok(response);
        }

        private static async Task<IResult> CadastrarConta(
             [FromBody] CadastrarUsuarioCommand command,
             [FromServices] ISender sender,
             CancellationToken cancellationToken)
        {
            var usuarioId = await sender.Send(command, cancellationToken);

            return Results.Created();
        }
    }
}
