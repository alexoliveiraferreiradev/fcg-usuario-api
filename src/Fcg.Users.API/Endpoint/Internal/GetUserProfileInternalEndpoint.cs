using Fcg.User.API.Filters;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Fcg.User.API.Endpoint.Internal
{
    public static class GetUserProfileInternalEndpoint
    {
        public static void MapGetUserProfileInternal(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/internal/users/{id:guid}", async (
                Guid id,
                IUserQueryRepository userQueryRepository,
                [FromHeader(Name = "x-internal-api-key")] string apiKey,
                IConfiguration configuration
                ) =>
            {
                var validApiKey = configuration["InternalSecrets:ServiceApiKey"];
                if(apiKey != validApiKey)
                {
                    return Results.Unauthorized();
                }

                var user = await userQueryRepository.GetUserInfoByIdAysnc(id);
                if(string.IsNullOrEmpty(user.NomeUsuario) && (string.IsNullOrEmpty(user.EmailUsuario)))
                {
                    return Results.NotFound();  
                }

                return Results.Ok(
                    new
                    {
                        UserId = id,
                        Name = user.NomeUsuario,
                        Email = user.EmailUsuario,
                    });


            }).WithTags("Internal Integrations")
            .ExcludeFromDescription();
        }
    }
}

