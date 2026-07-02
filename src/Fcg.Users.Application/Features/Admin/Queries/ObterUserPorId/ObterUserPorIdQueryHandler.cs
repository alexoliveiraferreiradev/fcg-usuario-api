using Dapper;
using Fcg.Users.Application.Features.Users.Responses;
using MediatR;
using System.Data;

namespace Fcg.Users.Application.Features.Admin.Queries.ObterUserPorId
{
    public class ObterUserPorIdQueryHandler : IRequestHandler<ObterUserPorIdQuery, UserResponse?>
    {
        private readonly IDbConnection _dbConnection;

        public ObterUserPorIdQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<UserResponse?> Handle(ObterUserPorIdQuery request, CancellationToken cancellationToken)
        {
            const string sql = @"SELECT 
                                 Id,
                                 NomeUser as Nome,
                                 EmailUser as Email,
                                 Ativo,
                                 Perfil as PerfilUser,
                                 DataAlteracao,
                                 MotivoDesativacao   
                                 FROM Users 
                                 where Id = @IdUser";

            var User = await _dbConnection.QueryFirstOrDefaultAsync<UserResponse>(sql, new { IdUser = request.Id });
            return User;
        }
    }
}
