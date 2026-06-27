using Fcg.Usuarios.Application.Features.Admin.Commands.DesativarUsuario;
using Fcg.Usuarios.Application.Features.Admin.Commands.PromoverUsuarioParaAdmin;
using Fcg.Usuarios.Application.Features.Admin.Commands.ReativarConta;
using Fcg.Usuarios.Application.Features.Admin.Commands.RebaixarUsuarioParaJogador;
using Fcg.Usuarios.Application.Features.Admin.Queries.ObterTodosUsuarios;
using Fcg.Usuarios.Application.Features.Admin.Queries.ObterUsuarioPorId;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using Fcg.Usuarios.Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fcg.Usuario.API.Endpoint.Admin
{
    public static class GerenciaUsuarioEndpoint
    {
        public static void MapGerenciaUsuarioEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/admin").WithTags("Gerenciamento de Usuários").RequireAuthorization();
            group.MapPut("desativar-usuario/{id:guid}", DesativaUsuario)
                 .Produces(StatusCodes.Status204NoContent)
                 .Produces(StatusCodes.Status400BadRequest)
                 .Produces(StatusCodes.Status401Unauthorized);

            group.MapPut("promover-para-admin/{id:guid}", PromoveUsuarioParaAdmin)
                 .Produces(StatusCodes.Status204NoContent)
                 .Produces(StatusCodes.Status404NotFound)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status400BadRequest);

            group.MapPut("reativar-jogador/{id:guid}", ReativaUsuario)
                 .Produces(StatusCodes.Status204NoContent)
                 .Produces(StatusCodes.Status401Unauthorized)
                 .Produces(StatusCodes.Status400BadRequest);

            group.MapPut("rebaixar-jogador/{id:guid}", RebaixarUsuario)
                 .Produces(StatusCodes.Status204NoContent)
                 .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("obter-todos", ObterTodos)
                 .Produces<IEnumerable<UsuarioResponse>>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("obter-usuario/{id:guid}", ObterUsuarioPorId)
                 .Produces<UsuarioResponse>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status404NotFound)
                 .Produces(StatusCodes.Status400BadRequest);
        }

        private static async Task<IResult> DesativaUsuario(
           [FromRoute] Guid id,
           [FromServices] ISender sender,
           [FromBody] MotivoDesativacao motivoDesativacao,
           ClaimsPrincipal user,
           CancellationToken cancellationToken)
        {
            var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserIdClaim))
            {
                return Results.Unauthorized();
            }

            var currentUserId = Guid.Parse(currentUserIdClaim);

            var desativarUsuarioCommand = new DesativarUsuarioCommand(id, currentUserId, motivoDesativacao);

            await sender.Send(desativarUsuarioCommand, cancellationToken);

            return Results.NoContent();
        }


        private static async Task<IResult> PromoveUsuarioParaAdmin(
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

            var usuarioAPromover = new PromoverUsuarioParaAdminCommand(id, adminId);

            var response = await sender.Send(usuarioAPromover, cancellationToken);

            return Results.NoContent();
        }


        private static async Task<IResult> ReativaUsuario(
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


        private static async Task<IResult> RebaixarUsuario(
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

            var rebaixarJogadorCommand = new RebaixarUsuarioParaJogadorCommand(id, currentUserId);

            await sender.Send(rebaixarJogadorCommand, cancellationToken);

            return Results.NoContent();
        }

        private static async Task<IResult> ObterTodos(
            [FromServices] ISender sender,
             CancellationToken CancellationToken)
        {
            var query = new ObterTodosUsuariosQuery();
            var usuarios = await sender.Send(query, CancellationToken);

            if (usuarios == null || !usuarios.Any())
                return Results.Ok(Enumerable.Empty<UsuarioResponse>);

            return Results.Ok(usuarios);
        }

        private static async Task<IResult> ObterUsuarioPorId(
            [FromRoute] Guid id,
            [FromServices] ISender sender,
            CancellationToken CancellationToken)
        {
            var query = new ObterUsuarioPorIdQuery(id);
            var usuario = await sender.Send(query, CancellationToken);
            if (usuario == null)
                return Results.Ok(Enumerable.Empty<UsuarioResponse>());
            
            return Results.Ok(usuario);
        }
    }
}

