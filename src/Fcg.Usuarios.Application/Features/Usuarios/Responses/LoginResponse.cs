using Fcg.Usuarios.Application.Features.Token;
using Fcg.Usuarios.Domain.Enum;

namespace Fcg.Usuarios.Application.Features.Usuarios.Responses
{
    public class LoginResponse
    {
        public string AcessToken { get; set; }
        public double ExpiresIn { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public IEnumerable<ClaimResponse> Claims { get; set; }
        public TipoUsuario PerfilUsuario { get; set; }
    }
}
