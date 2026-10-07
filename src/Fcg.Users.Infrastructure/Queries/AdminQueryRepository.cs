using Dapper;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Responses;
using System.Data;

namespace Fcg.Users.Infrastructure.Queries
{
    public class AdminQueryRepository : IAdminQueryRepository
    {
        private readonly IDbConnection _dbConnection;

        public AdminQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync(CancellationToken cancellation)
        {
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

            return await _dbConnection.QueryAsync<UserResponse>(sql);
        }

        public async Task<UserResponse?> GetUserByIdAsync(Guid userId, CancellationToken cancellation)
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

            return await _dbConnection.QueryFirstOrDefaultAsync<UserResponse>(sql, new { IdUser = userId });
        }
    }
}
