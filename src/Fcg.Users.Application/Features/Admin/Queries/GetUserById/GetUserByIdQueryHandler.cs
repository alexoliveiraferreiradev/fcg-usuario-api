using Dapper;
using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Responses;
using MediatR;
using System.Data;

namespace Fcg.Users.Application.Features.Admin.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponse?>
    {
        private readonly IAdminQueryRepository _adminQueryRepository;

        public GetUserByIdQueryHandler(IAdminQueryRepository adminQueryRepository)
        {
            _adminQueryRepository = adminQueryRepository;
        }

        public async Task<UserResponse?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return await _adminQueryRepository.GetUserByIdAsync(request.Id,cancellationToken);    
        }
    }
}
