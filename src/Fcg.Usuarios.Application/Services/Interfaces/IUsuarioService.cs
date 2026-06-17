

using Fcg.Usuarios.Application.Dtos;
using Fcg.Usuarios.Application.Features.Usuarios;

namespace Fcg.Usuarios.Applicatio.Services.Interfaces
{
    public interface IUsuarioService  
    {
        Task<UsuarioResponse> PromoverParaAdmin(Guid id);
        Task<UsuarioResponse> RebaixarParaJogador(Guid id,Guid idOperador);
        Task<UsuarioResponse> CadastrarUsuario(CriaUsuarioRequest request);
        Task<UsuarioResponse> AtualizarUsuario(Guid usuarioId, UpdateUsuarioRequest updateUsuarioRequest);
        Task Desativar(DesativaUsuarioRequest deleteUsuarioRequest,Guid idOperador);
        Task DesativarConta(Guid id);
        Task Reativar(Guid usuarioId);
        Task<UsuarioResponse> Autenticar(LoginRequest loginRequest);
        Task<UsuarioResponse> ObterPorId(Guid usuarioId);
        Task<IEnumerable<UsuarioResponse>> ObterTodos();
        Task<UsuarioResponse?> ObterPorEmail(string emailUsuario);

    }
}