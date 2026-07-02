using Fcg.Users.Application.Features.Users.Commands.AuthenticateUser;
using Fcg.Users.Application.Features.Users.Commands.RegisterUser;
using Fcg.Users.Application.Features.Users.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.User.API.Endpoint.User
{
    public static class AcessoUserEndpoint
    {
        public static void MapAcessoUserEndpoint(this WebApplication app)
        {
            var group = app.MapGroup("/api/User").WithTags("Autenticação do Usuário");

            group.MapPost("/login", AutenticarConta)
                .Produces<LoginResponse>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound);

            group.MapPost("/cadastrar", CadastrarConta)
                .Produces(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest);
        }

        private static async Task<IResult> AutenticarConta(
            [FromBody] AuthenticateUserCommand command,
            [FromServices] ISender mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(command, cancellationToken);

            if (response == null)
                return Results.NotFound(new { Mensagem = "Usuário não encontrado." });

            return Results.Ok(response);
        }

        private static async Task<IResult> CadastrarConta(
             [FromBody] RegisterUserCommand command,
             [FromServices] ISender sender,
             CancellationToken cancellationToken)
        {
            var UserId = await sender.Send(command, cancellationToken);

            return Results.Created();
        }
    }
}
