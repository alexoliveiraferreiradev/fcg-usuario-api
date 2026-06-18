using Dapper;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using MediatR;
using System.Data;

namespace Fcg.Usuarios.Application.Features.Usuarios.Queries.ObterUsuarioPorId
{
    public class ObterUsuarioPorIdQueryHandler : IRequestHandler<ObterUsuarioPorIdQuery, UsuarioResponse?>
    {
        private readonly IDbConnection _dbConnection;

        public ObterUsuarioPorIdQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<UsuarioResponse?> Handle(ObterUsuarioPorIdQuery request, CancellationToken cancellationToken)
        {
            const string sql = @"SELECT 
                                 Id,
                                 NomeUsuario as Nome,
                                 EmailUsuario as Email,
                                 Ativo,
                                 Perfil as PerfilUsuario,
                                 DataAlteracao,
                                 MotivoDesativacao   
                                 FROM USUARIOS 
                                 where Id = @IdUsuario";

            var usuario = await _dbConnection.QueryFirstOrDefaultAsync<UsuarioResponse>(sql, new { IdUsuario = request.Id });
            return usuario;
        }
    }
}
