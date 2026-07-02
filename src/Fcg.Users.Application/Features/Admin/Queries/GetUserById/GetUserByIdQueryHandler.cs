using Dapper;
using Fcg.Users.Application.Features.Users.Responses;
using MediatR;
using System.Data;

namespace Fcg.Users.Application.Features.Admin.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponse?>
    {
        private readonly IDbConnection _dbConnection;

        public GetUserByIdQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<UserResponse?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            const string sql = @"SELECT 
                                 Id,
                                 Name as Name,
                                 Email as Email,
                                 IsActive,
                                 Role as PerfilUser,
                                 UpdatedAt,
                                 DeactivationReason   
                                 FROM Users 
                                 where Id = @IdUser";

            var User = await _dbConnection.QueryFirstOrDefaultAsync<UserResponse>(sql, new { IdUser = request.Id });
            return User;
        }
    }
}
