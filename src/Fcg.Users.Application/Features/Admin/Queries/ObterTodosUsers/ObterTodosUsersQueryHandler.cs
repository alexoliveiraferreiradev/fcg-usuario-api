using Dapper;
using Fcg.Users.Application.Features.Users.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Fcg.Users.Application.Features.Admin.Queries.ObterTodosUsers
{
    public class ObterTodosUsersQueryHandler : IRequestHandler<ObterTodosUsersQuery, IEnumerable<UserResponse>>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<ObterTodosUsersQueryHandler> _logger;

        public ObterTodosUsersQueryHandler(IDbConnection dbConnection,
            ILogger<ObterTodosUsersQueryHandler> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public async Task<IEnumerable<UserResponse>> Handle(ObterTodosUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Solicitação de listagem geral de usuários recebida.");
            const string sql = @"
                    SELECT 
                        Id, 
                        NomeUser AS Nome, 
                        EmailUser AS Email, 
                        Ativo,
                        Perfil AS PerfilUser,
                        DataAlteracao,
                        MotivoDesativacao
                    FROM Users 
                    ORDER BY DataCadastro DESC";

            var Users = await _dbConnection.QueryAsync<UserResponse>(sql);

            var resultado = Users.ToList();

            if(!resultado.Any())
            {
                _logger.LogInformation("Consulta finalizada. A base de usuários está vazia.");
            }
            else
            {
                _logger.LogInformation("Listagem de usuários realizada com sucesso. Total de registros: {QuantidadeUsers}",
                    resultado.Count);
            }

            return resultado;
        }
    }
}
