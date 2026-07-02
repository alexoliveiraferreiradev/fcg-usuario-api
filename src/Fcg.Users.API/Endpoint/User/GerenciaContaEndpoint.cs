using Fcg.Users.Application.Features.Users.Commands.AtualizarUser;
using Fcg.Users.Application.Features.Users.Commands.AutenticarUser;
using Fcg.Users.Application.Features.Users.Commands.CadastrarUser;
using Fcg.Users.Application.Features.Users.Commands.DesativarConta;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fcg.User.API.Endpoint.User
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
           [FromBody] AtualizarUserCommand command,
           [FromServices] ISender mediator,
           CancellationToken cancellationToken,
           ClaimsPrincipal user)
        {
            var currentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentId))
            {
                return Results.Unauthorized();
            }

            var UserId = Guid.Parse(currentId);

            var atualizarCommand = command with { UserId = UserId };

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

            var UserId = Guid.Parse(currentId);

            var desativarContaCommand = command with { Id = UserId };

            await mediator.Send(desativarContaCommand);

            return Results.NoContent();

        }
}
}
