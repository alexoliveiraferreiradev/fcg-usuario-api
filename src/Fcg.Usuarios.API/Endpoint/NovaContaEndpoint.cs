using Fcg.Usuarios.Application.Features.Usuarios.Commands.CadastrarUsuario;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Usuario.API.Endpoint
{
    public static class NovaContaEndpoint
    {
        public static void MapNovaContaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/novaconta").AllowAnonymous().WithTags("NovaConta");

            group.MapPost("cadastrar", async (
                [FromBody] CadastrarUsuarioCommand command,
                [FromServices] ISender mediator,
                CancellationToken cancellationToken) =>
            {
                if (command.Senha != command.ConfirmacaoSenha)
                    return Results.BadRequest("A senha e a confirmação de senha devem ser iguais.");

                var usuarioId = await mediator.Send(command, cancellationToken);

                return Results.Created();
            })
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
