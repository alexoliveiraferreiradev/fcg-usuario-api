using Fcg.Users.Application.Features.Users.Commands.DeactivateAccount;
using Fcg.Users.Application.Features.Users.Commands.UpdateUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fcg.User.API.Endpoint.User
{
    public static class ManageAccountEndpoint
    {
        public static void MapManageAccountEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/my-account").WithTags("Minha Conta").RequireAuthorization();

            group.MapPut("/update", UpdateAccount)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Atualiza os dados cadastrais do usuário autenticado.")
                .WithDescription("""
                    Permite que o próprio usuário autenticado altere seu nome, senha e confirmação de senha. 
                    O identificador do usuário (UserId) é obtido de forma segura diretamente a partir das claims do token JWT.
                    
                    * **Name:** Novo nome do usuário.
                    * **Password:** Nova senha de acesso (mínimo de 8 caracteres, com requisitos de complexidade).
                    * **ConfirmPassword:** Confirmação idêntica da nova senha.
                 """)
                .WithName("UpdateUserAccount");

            group.MapPut("/deactivate-conta", DeactivateAccount)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status204NoContent)
                .WithSummary("Desativa a conta do usuário autenticado.")
                .WithDescription("""
                    Realiza a desativação lógica da conta do próprio usuário autenticado. 
                    O identificador do usuário é extraído de forma segura diretamente das claims do token JWT.
                 """)
                .WithName("DeactivateUserAccount");


        }
        private static async Task<IResult> UpdateAccount(
           [FromBody] UpdateUserCommand command,
           [FromServices] ISender mediator,
           CancellationToken cancellationToken,
           ClaimsPrincipal user)
        {
            var currentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentId))
            {
                return Results.Unauthorized();
            }

            var userId = Guid.Parse(currentId);

            var atualizarCommand = command with { UserId = userId };

            var response = await mediator.Send(atualizarCommand, cancellationToken);

            if (response == null)
                return Results.NotFound(new { Mensagem = "Usuário não encontrado." });

            return Results.Ok(response);
        }

        private static async Task<IResult> DeactivateAccount(
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

            var userId = Guid.Parse(currentId);

            var desativarContaCommand = command with { Id = userId };

            await mediator.Send(desativarContaCommand);

            return Results.NoContent();

        }
    }
}
