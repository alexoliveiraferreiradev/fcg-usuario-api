using Fcg.Usuarios.Application.Features.Token;
using Fcg.Usuarios.Application.Features.Usuarios;

namespace Fcg.Usuarios.Application.Common.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResult> GerarToken(UsuarioResponse usuario);
    }
}
