using Fcg.Usuarios.Application.Features.Usuarios.Commands.AutenticarUsuario;
using Fcg.Usuarios.Application.Features.Usuarios.Commands.CadastrarUsuario;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.Usuario.API.Endpoint.Usuario
{
    public static class CadastrarContaEndpoint
    {
        public static void MapNovaContaEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/novaconta").AllowAnonymous().WithTags("Autenticação do Usuário");

            group.MapPost("cadastrar", async (
                [FromBody] CadastrarUsuarioCommand command,
                [FromServices] ISender mediator,
                CancellationToken cancellationToken) =>
            {
                var usuarioId = await mediator.Send(command, cancellationToken);

                return Results.Created();
            })
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .AddEndpointFilter<ValidationFilter<CadastrarUsuarioCommand>>();
        }
    }
}
