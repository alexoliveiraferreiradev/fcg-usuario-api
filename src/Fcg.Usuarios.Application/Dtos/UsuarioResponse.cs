using Fcg.Usuarios.Domain.Enum;

namespace Fcg.Usuarios.Application.Dtos
{
    public class UsuarioResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }
        public TipoUsuario PerfilUsuario {  get; set; }
        public DateTime DataAlteracao { get; set; }
        public MotivoDesativacao? MotivoDesativacao { get; private set; }
    }
}
