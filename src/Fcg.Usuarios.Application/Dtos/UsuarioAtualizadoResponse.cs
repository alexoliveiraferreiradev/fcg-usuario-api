namespace Fcg.Usuarios.Application.Dtos
{
    public class UsuarioAtualizadoResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}
