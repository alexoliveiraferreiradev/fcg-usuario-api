using Dapper;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Fcg.Users.Application.Features.Admin.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserResponse>>
    {        
        private readonly ILogger<GetAllUsersQueryHandler> _logger;
        private readonly IAdminQueryRepository _adminQueryRepository;

        public GetAllUsersQueryHandler(ILogger<GetAllUsersQueryHandler> logger,
            IAdminQueryRepository adminQueryRepository)
        {
            _logger = logger;
            _adminQueryRepository = adminQueryRepository;
        }

        public async Task<IEnumerable<UserResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Solicitação de listagem geral de usuários recebida.");

            var resultado = await _adminQueryRepository.GetAllUsersAsync(cancellationToken);
                       

            if(!resultado.Any())
            {
                _logger.LogInformation("Consulta finalizada. A base de usuários está vazia.");
            }
            else
            {
                _logger.LogInformation("Listagem de usuários realizada com sucesso. Total de registros: {QuantidadeUsers}",
                    resultado.ToList().Count);
            }

            return resultado.ToList();
        }
    }
}
