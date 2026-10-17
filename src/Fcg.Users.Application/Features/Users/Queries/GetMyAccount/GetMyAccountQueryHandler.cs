using Fcg.Users.Application.Common.Interfaces;
using Fcg.Users.Application.Features.Users.Responses;
using MediatR;

namespace Fcg.Users.Application.Features.Users.Queries.GetMyAccount
{
    public class GetMyAccountQueryHandler : IRequestHandler<GetMyAccountQuery, UserAccountResponse?>
    {
        private readonly IUserQueryRepository _userQueryRepository;

        public GetMyAccountQueryHandler(IUserQueryRepository userQueryRepository)
        {
            _userQueryRepository = userQueryRepository;
        }

        public async Task<UserAccountResponse?> Handle(GetMyAccountQuery request, CancellationToken cancellationToken)
        {
            return await _userQueryRepository.GetUserByIdAsync(request.UserId, cancellationToken);
        }
    }
}
