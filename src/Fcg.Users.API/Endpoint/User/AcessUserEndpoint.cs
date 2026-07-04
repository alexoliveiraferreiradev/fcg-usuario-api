using Fcg.Users.Application.Features.Users.Commands.AuthenticateUser;
using Fcg.Users.Application.Features.Users.Commands.RegisterUser;
using Fcg.Users.Application.Features.Users.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Fcg.User.API.Filters;

namespace Fcg.User.API.Endpoint.User
{
    public static class AcessUserEndpoint
    {
        public static void MapAcessUserEndpoint(this WebApplication app)
        {
            var group = app.MapGroup("/api/user").WithTags("Autenticação do Usuário").AllowAnonymous();

            group.MapPost("/sign-in", AuthenticateUser)
                .AddEndpointFilter<ValidationFilter<AuthenticateUserCommand>>()
                .Produces<LoginResponse>(StatusCodes.Status200OK)
                .ProducesValidationProblem()
                .Produces(StatusCodes.Status401Unauthorized)
                .WithSummary("Autentica um usuário e gera um token de acesso.")
                .WithDescription("""
                    * **Validação de E-mail:** Obrigatório, formato válido, verificação de duplicidade.
                    * **Validação de Senha:** Mínimo 8 caracteres (A-z, 0-9, @#$), confirmação idêntica.
        
                    Exemplo de requisição:
                        ```json
                    POST /login
                        {
                            "email": "usuario@exemplo.com",
                            "senha": "SenhaForte123!"
                        }
                        ```
                 """)
                .WithName("Autheticate New User");

            group.MapPost("/register", RegisterUser)
                .AddEndpointFilter<ValidationFilter<RegisterUserCommand>>()
                .Produces(StatusCodes.Status201Created)
                .ProducesValidationProblem()
                .Produces(StatusCodes.Status409Conflict)
                .WithSummary("Registra um novo usuário no sistema.")
                .WithDescription("""
                    Cadastra um novo usuário no banco de dados. Realiza validações de formato de e-mail, força de senha e confirmação de senha. Retorna o identificador único (GUID) do usuário cadastrado em caso de sucesso.
                    
                    * **Nome:** Nome completo do usuário.
                    * **E-mail:** Endereço de e-mail único para o cadastro.
                    * **Senha:** Senha de acesso (mínimo de 8 caracteres, contendo letras maiúsculas, minúsculas, números e caracteres especiais).
                    * **Confirmação de Senha:** Confirmação idêntica da senha digitada.
                 """)
                .WithName("Register New User");
        }



        private static async Task<IResult> AuthenticateUser(
            [FromBody] AuthenticateUserCommand command,
            [FromServices] ISender mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(command, cancellationToken);

            if (response == null)
                return Results.NotFound(new { Mensagem = "Usuário não encontrado." });

            return Results.Ok(response);
        }

        private static async Task<IResult> RegisterUser(
             [FromBody] RegisterUserCommand command,
             [FromServices] ISender sender,
             CancellationToken cancellationToken)
        {
            var UserId = await sender.Send(command, cancellationToken);

            return Results.Created();
        }
    }
}
