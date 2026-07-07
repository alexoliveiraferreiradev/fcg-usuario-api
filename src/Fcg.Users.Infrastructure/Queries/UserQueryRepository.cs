using Dapper;
using Fcg.Users.Application.Common.Interfaces;
using System.Data;

namespace Fcg.Users.Infrastructure.Queries
{
    public class UserQueryRepository : IUserQueryRepository
    {
        private readonly IDbConnection _dbConnection;
        public UserQueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;   
        }
        public async Task<(string NomeUsuario, string EmailUsuario)> GetUserInfoByIdAysnc(Guid userId)
        {
            const string sql = @"SELECT Name,Email FROM USERS WHERE ID = @ID";
            return await _dbConnection.QueryFirstAsync<(string nome, string email)>(sql, new {Id =  userId});   
        }
    }
}
