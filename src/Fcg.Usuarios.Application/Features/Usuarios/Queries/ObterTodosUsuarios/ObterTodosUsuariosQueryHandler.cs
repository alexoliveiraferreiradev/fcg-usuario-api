using Dapper;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using MediatR;
using System.Data;

namespace Fcg.Usuarios.Application.Features.Usuarios.Queries.ObterTodosUsuarios
{
    public class ObterTodosUsuariosQueryHandler : IRequestHandler<ObterTodosUsuariosQuery, IEnumerable<UsuarioResponse>>
    {
        private readonly IDbConnection _dbConnection;

        public ObterTodosUsuariosQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<UsuarioResponse>> Handle(ObterTodosUsuariosQuery request, CancellationToken cancellationToken)
        {
            const string sql = @"
                    SELECT 
                        Id, 
                        NomeUsuario AS Nome, 
                        EmailUsuario AS Email, 
                        Ativo, -- Faltava aqui!
                        Perfil AS PerfilUsuario,
                        DataAlteracao,
                        MotivoDesativacao
                    FROM Usuarios 
                    ORDER BY DataCadastro DESC";

            var usuarios = await _dbConnection.QueryAsync<UsuarioResponse>(sql);

            return usuarios;
        }
    }
}
