using Dapper;
using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Fcg.Usuarios.Application.Features.Admin.Queries.ObterTodosUsuarios
{
    public class ObterTodosUsuariosQueryHandler : IRequestHandler<ObterTodosUsuariosQuery, IEnumerable<UsuarioResponse>>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<ObterTodosUsuariosQueryHandler> _logger;

        public ObterTodosUsuariosQueryHandler(IDbConnection dbConnection,
            ILogger<ObterTodosUsuariosQueryHandler> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public async Task<IEnumerable<UsuarioResponse>> Handle(ObterTodosUsuariosQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Solicitação de listagem geral de usuários recebida.");
            const string sql = @"
                    SELECT 
                        Id, 
                        NomeUsuario AS Nome, 
                        EmailUsuario AS Email, 
                        Ativo,
                        Perfil AS PerfilUsuario,
                        DataAlteracao,
                        MotivoDesativacao
                    FROM Usuarios 
                    ORDER BY DataCadastro DESC";

            var usuarios = await _dbConnection.QueryAsync<UsuarioResponse>(sql);

            var resultado = usuarios.ToList();

            if(!resultado.Any())
            {
                _logger.LogInformation("Consulta finalizada. A base de usuários está vazia.");
            }
            else
            {
                _logger.LogInformation("Listagem de usuários realizada com sucesso. Total de registros: {QuantidadeUsuarios}",
                    resultado.Count);
            }

            return resultado;
        }
    }
}
