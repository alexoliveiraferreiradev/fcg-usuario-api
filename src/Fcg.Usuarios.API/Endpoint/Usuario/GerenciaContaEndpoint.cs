using Fcg.Usuarios.Application.Features.Usuarios.Commands.AtualizarUsuario;
using Fcg.Usuarios.Application.Features.Usuarios.Commands.AutenticarUsuario;
using Fcg.Usuarios.Application.Features.Usuarios.Commands.CadastrarUsuario;
using Fcg.Usuarios.Application.Features.Usuarios.Commands.DesativarConta;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fcg.Usuario.API.Endpoint.Usuario
{
    public static class GerenciaContaEndpoint
    {
        public static void MapGerenciaContaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/minha-conta").WithTags("Minha Conta").RequireAuthorization();
           
            group.MapPut("atualizar", AtualizaConta)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound);
         
            group.MapPut("desativar-conta", DesativarConta)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status204NoContent);


        }
        private static async Task<IResult> AtualizaConta(
           [FromBody] AtualizarUsuarioCommand command,
           [FromServices] ISender mediator,
           CancellationToken cancellationToken,
           ClaimsPrincipal user)
        {
            var currentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentId))
            {
                return Results.Unauthorized();
            }

            var usuarioId = Guid.Parse(currentId);

            var atualizarCommand = command with { UsuarioId = usuarioId };

            var response = await mediator.Send(atualizarCommand, cancellationToken);

            if (response == null)
                return Results.NotFound(new { Mensagem = "Usuário não encontrado." });

            return Results.Ok(response);
        }

        private static async Task<IResult> DesativarConta(
            [FromBody] DesativarContaCommand command,
            [FromServices] ISender mediator,
            CancellationToken cancellationToken,
            ClaimsPrincipal user)
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

        }
}
}
