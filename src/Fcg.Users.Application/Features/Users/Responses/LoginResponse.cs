using Fcg.Users.Application.Features.Users.Responses.Token;
using Fcg.Users.Domain.Enum;

namespace Fcg.Users.Application.Features.Users.Responses
{
    public class LoginResponse
    {
        public string AcessToken { get; set; }
        public double ExpiresIn { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<ClaimResponse> Claims { get; set; }
        public UserRole PerfilUser { get; set; }
    }
}
