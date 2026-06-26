using Fcg.Usuarios.Application.Features.Usuarios.Responses;
using Fcg.Usuarios.Application.Features.Usuarios.Responses.Token;

namespace Fcg.Usuarios.Application.Common.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResult> GerarToken(UsuarioResponse usuario);
    }
}
