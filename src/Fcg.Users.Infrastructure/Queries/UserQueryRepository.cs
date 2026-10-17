using Dapper;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Responses;
using System.Data;

namespace Fcg.Users.Infrastructure.Queries
{
    internal class UserQueryRepository : IUserQueryRepository
    {
        private readonly IDbConnection _dbConnection;

        public UserQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <returns>null quando o usuário do token não existe mais (o endpoint responde 404).</returns>
        public async Task<UserAccountResponse?> GetUserByIdAsync(Guid userId, CancellationToken cancellation)
        {
            const string sql = @"
                    SELECT 
                        Name AS Name, 
                        Email AS Email
                    FROM Users 
                    WHERE Id = @UserId";

            return await _dbConnection.QuerySingleOrDefaultAsync<UserAccountResponse>(
                new CommandDefinition(sql, new { UserId = userId }, cancellationToken: cancellation));
        }
    }
}
