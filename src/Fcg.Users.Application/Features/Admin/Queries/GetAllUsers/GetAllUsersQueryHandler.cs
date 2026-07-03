using Dapper;
using Fcg.Users.Application.Features.Users.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Fcg.Users.Application.Features.Admin.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserResponse>>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<GetAllUsersQueryHandler> _logger;

        public GetAllUsersQueryHandler(IDbConnection dbConnection,
            ILogger<GetAllUsersQueryHandler> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public async Task<IEnumerable<UserResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Solicitação de listagem geral de usuários recebida.");
            const string sql = @"
                    SELECT 
                        Id, 
                        Name AS Name, 
                        Email AS Email, 
                        IsActive,
                        Role AS PerfilUser,
                        UpdatedAt,
                        DeactivationReason
                    FROM Users 
                    ORDER BY CreatedAt DESC";

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
