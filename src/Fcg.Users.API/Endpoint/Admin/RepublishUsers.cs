using Fcg.Users.Application.Features.Admin.Commands.RepublishUsers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.User.API.Endpoint.Admin
{
    public static class RepublishUsers
    {
        public static void MapRepublishUsersEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/admin/users")
                .WithTags("Admin - Gerenciamento de Usuários").RequireAuthorization("AdminOnly");

            group.MapPost("/republish", RepublishAllUsers)
                 .Produces<int>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .WithSummary("Republica todos os usuários para os demais serviços.")
                 .WithDescription("""
                    Publica novamente, no message broker, o evento de integração de cada usuário cadastrado.
                    Útil quando algum consumidor (ex.: a função serverless) não estava disponível no momento do cadastro.
                    Retorna a quantidade de usuários republicados.
                 """)
                 .WithName("AdminRepublishUsers");
        }

        private static async Task<IResult> RepublishAllUsers(
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            var total = await sender.Send(new RepublishUsersCommand(), cancellationToken);

            return Results.Ok(total);
        }
    }
}
