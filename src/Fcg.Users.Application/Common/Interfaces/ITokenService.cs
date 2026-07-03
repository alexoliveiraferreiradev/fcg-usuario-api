using Fcg.Users.Application.Features.Users.Responses;
using Fcg.Users.Application.Features.Users.Responses.Token;

namespace Fcg.Users.Application.Common.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResult> GenerateToken(UserResponse User);
    }
}
