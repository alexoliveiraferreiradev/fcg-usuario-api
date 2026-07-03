using Fcg.Users.Application.Features.Admin.Commands.DeactiveUser;
using Fcg.Users.Application.Features.Admin.Commands.DemoteUserToPlayer;
using Fcg.Users.Application.Features.Admin.Commands.PromoteUserToAdmin;
using Fcg.Users.Application.Features.Admin.Commands.ReactiveAccount;
using Fcg.Users.Application.Features.Admin.Queries.GetAllUsers;
using Fcg.Users.Application.Features.Admin.Queries.GetUserById;
using Fcg.Users.Application.Features.Users.Responses;
using Fcg.Users.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fcg.User.API.Endpoint.Admin
{
    public static class GerenciaUserEndpoint
    {
        public static void MapGerenciaUserEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/admin/users")
                .WithTags("Admin - Gerenciamento de Usuários").RequireAuthorization("AdminOnly");
           
            group.MapPut("/{id:guid}/deactivate-user", DeactivateUser)
                 .Produces(StatusCodes.Status204NoContent)
                 .Produces(StatusCodes.Status400BadRequest)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .WithSummary("Desativa a conta de um usuário (Administrador).")
                 .WithDescription("""
                    Permite que um administrador desative temporariamente ou permanentemente a conta de um usuário. 
                    Requer a indicação de um motivo do tipo `DeactivationReason` no corpo da requisição. O ID do administrador operador é extraído das claims do token JWT.
                    
                    Motivos de desativação aceitos (DeactivationReason):
                    * `1` - UserRequested (Solicitado pelo usuário)
                    * `2` - Inactivity (Inatividade prolongada)
                    * `3` - TermsViolation (Violação dos Termos de Uso)
                    * `4` - InappropriateBehavior (Comportamento tóxico ou inadequado)
                    * `5` - FraudOrCheating (Uso de trapaças/Cheats/Bots)
                    * `6` - DuplicateAccount (Duplicidade de conta)
                    * `99` - Other (Outros motivos)
                 """)
                 .WithName("AdminDeactivateUser");

            group.MapPut("/{id:guid}/promote", PromoteToAdmin)
                 .Produces(StatusCodes.Status204NoContent)
                 .Produces(StatusCodes.Status404NotFound)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status400BadRequest)
                 .WithSummary("Promove um usuário para a função de Administrador.")
                 .WithDescription("""
                    Concede o papel de Administrador a um usuário existente através de seu ID. 
                    Com isso, o usuário passa a ter privilégios elevados de gerenciamento na API.
                 """)
                 .WithName("AdminPromoteUserToAdmin");

            group.MapPut("/{id:guid}/reactivate", ReactivateUser)
                 .Produces(StatusCodes.Status204NoContent)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status400BadRequest)
                 .WithSummary("Reativa a conta de um usuário desativado.")
                 .WithDescription("""
                    Restaura o status ativo de um usuário que teve sua conta desativada previamente, permitindo que ele volte a logar no sistema.
                 """)
                 .WithName("AdminReactivateUser");

            group.MapPut("/{id:guid}/demote", DemoteUser)
                 .Produces(StatusCodes.Status204NoContent)
                 .Produces(StatusCodes.Status400BadRequest)
                 .WithSummary("Rebaixa um usuário de Administrador para Jogador comum.")
                 .WithDescription("""
                    Remove a função de Administrador de um usuário, rebaixando seu perfil de acesso para Jogador. 
                    Atenção: Não é permitido auto-rebaixamento (um administrador rebaixar a si próprio logado).
                 """)
                 .WithName("AdminDemoteUserToPlayer");

            group.MapGet("", GetAllUsers)
                 .Produces<IEnumerable<UserResponse>>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status400BadRequest)
                 .WithSummary("Lista todos os usuários cadastrados.")
                 .WithDescription("""
                    Retorna uma lista de todos os usuários registrados no sistema, contendo detalhes de perfil, status da conta e data de cadastro.
                 """)
                 .WithName("AdminGetAllUsers");

            group.MapGet("/{id:guid}", GetUserById)
                 .Produces<UserResponse>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status404NotFound)
                 .Produces(StatusCodes.Status400BadRequest)
                 .WithSummary("Obtém detalhes de um usuário por ID.")
                 .WithDescription("""
                    Busca um usuário específico pelo seu identificador único (GUID) e retorna suas informações detalhadas.
                 """)
                 .WithName("AdminGetUserById");
        }

        private static async Task<IResult> DeactivateUser(
           [FromRoute] Guid id,
           [FromServices] ISender sender,
           [FromBody] DeactivationReason DeactivationReason,
           ClaimsPrincipal user,
           CancellationToken cancellationToken)
        {
            var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserIdClaim))
            {
                return Results.Unauthorized();
            }

            var currentUserId = Guid.Parse(currentUserIdClaim);

            var deactivateUserCommand = new DeactivateUserCommand(id, currentUserId, DeactivationReason);

            await sender.Send(deactivateUserCommand, cancellationToken);

            return Results.NoContent();
        }


        private static async Task<IResult> PromoteToAdmin(
                ClaimsPrincipal user,
                [FromRoute] Guid id,
                [FromServices] ISender sender,
                CancellationToken cancellationToken
            )
        {
            var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserIdClaim))
                return Results.Unauthorized();

            var adminId = Guid.Parse(currentUserIdClaim);

            var UserAPromover = new PromoteUserToAdminCommand(id, adminId);

            var response = await sender.Send(UserAPromover, cancellationToken);

            return Results.NoContent();
        }


        private static async Task<IResult> ReactivateUser(
            [FromRoute] Guid id,
            [FromServices] ISender sender,
            CancellationToken cancellationToken,
            ClaimsPrincipal user)
        {
            var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserIdClaim))
            {
                return Results.Unauthorized();
            }
            var reativarJogadorCommand = new ReactivateAccountCommand(id);

            await sender.Send(reativarJogadorCommand, cancellationToken);

            return Results.NoContent();
        }


        private static async Task<IResult> DemoteUser(
             [FromRoute] Guid id,
             [FromServices] ISender sender,
             CancellationToken cancellationToken,
             ClaimsPrincipal user)
        {
            var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var currentUserId = Guid.Parse(currentUserIdClaim);

            if (id == currentUserId)
            {
                return Results.BadRequest("Você não possui permissão para rebaixar a própria conta enquanto logado");
            }

            var rebaixarJogadorCommand = new DemoteUserToPlayerCommand(id, currentUserId);

            await sender.Send(rebaixarJogadorCommand, cancellationToken);

            return Results.NoContent();
        }

        private static async Task<IResult> GetAllUsers(
            [FromServices] ISender sender,
             CancellationToken CancellationToken)
        {
            var query = new GetAllUsersQuery();
            var Users = await sender.Send(query, CancellationToken);

            if (Users == null || !Users.Any())
                return Results.Ok(Enumerable.Empty<UserResponse>);

            return Results.Ok(Users);
        }

        private static async Task<IResult> GetUserById(
            [FromRoute] Guid id,
            [FromServices] ISender sender,
            CancellationToken CancellationToken)
        {
            var query = new GetUserByIdQuery(id);
            var User = await sender.Send(query, CancellationToken);
            if (User == null)
                return Results.Ok(Enumerable.Empty<UserResponse>());
            
            return Results.Ok(User);
        }
    }
}

