using Fcg.Usuarios.Application.Dtos;
using MediatR;

namespace Fcg.Usuarios.Application.Features.Commands.AtualizarUsuario
{
    public class AtualizarUsuarioCommand : IRequest<UsuarioResponse>
    {
        public Guid UsuarioId { get; set; }
        public string NomeUsuario { get; set; }
        public string SenhaUsuario { get; set; }
        public string ConfirmacaoSenha { get; set; }
    }
}
