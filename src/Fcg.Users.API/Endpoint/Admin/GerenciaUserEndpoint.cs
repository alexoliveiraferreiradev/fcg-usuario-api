using Fcg.Users.Application.Features.Admin.Commands.DesativarUser;
using Fcg.Users.Application.Features.Admin.Commands.PromoverUserParaAdmin;
using Fcg.Users.Application.Features.Admin.Commands.ReativarConta;
using Fcg.Users.Application.Features.Admin.Commands.RebaixarUserParaJogador;
using Fcg.Users.Application.Features.Admin.Queries.ObterTodosUsers;
using Fcg.Users.Application.Features.Admin.Queries.ObterUserPorId;
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
            var group = app.MapGroup("api/admin").WithTags("Gerenciamento de Usuários").RequireAuthorization();
            group.MapPut("Deactivate-User/{id:guid}", DesativaUser)
                 .Produces(StatusCodes.Status204NoContent)
                 .Produces(StatusCodes.Status400BadRequest)
                 .Produces(StatusCodes.Status401Unauthorized);

            group.MapPut("promover-para-admin/{id:guid}", PromoveUserParaAdmin)
                 .Produces(StatusCodes.Status204NoContent)
                 .Produces(StatusCodes.Status404NotFound)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status400BadRequest);

            group.MapPut("Reactivate-Player/{id:guid}", ReativaUser)
                 .Produces(StatusCodes.Status204NoContent)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status400BadRequest);

            group.MapPut("rebaixar-Player/{id:guid}", RebaixarUser)
                 .Produces(StatusCodes.Status204NoContent)
                 .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("obter-todos", ObterTodos)
                 .Produces<IEnumerable<UserResponse>>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("obter-User/{id:guid}", ObterUserPorId)
                 .Produces<UserResponse>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status404NotFound)
                 .Produces(StatusCodes.Status400BadRequest);
        }

        private static async Task<IResult> DesativaUser(
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

            var desativarUserCommand = new DesativarUserCommand(id, currentUserId, DeactivationReason);

            await sender.Send(desativarUserCommand, cancellationToken);

            return Results.NoContent();
        }


        private static async Task<IResult> PromoveUserParaAdmin(
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

            var UserAPromover = new PromoverUserParaAdminCommand(id, adminId);

            var response = await sender.Send(UserAPromover, cancellationToken);

            return Results.NoContent();
        }


        private static async Task<IResult> ReativaUser(
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
            var reativarJogadorCommand = new ReativarContaCommand(id);

            await sender.Send(reativarJogadorCommand, cancellationToken);

            return Results.NoContent();
        }


        private static async Task<IResult> RebaixarUser(
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

        private static async Task<IResult> ObterTodos(
            [FromServices] ISender sender,
             CancellationToken CancellationToken)
        {
            var query = new GetAllUsersQuery();
            var Users = await sender.Send(query, CancellationToken);

            if (Users == null || !Users.Any())
                return Results.Ok(Enumerable.Empty<UserResponse>);

            return Results.Ok(Users);
        }

        private static async Task<IResult> ObterUserPorId(
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

