using Fcg.Users.Domain.Enum;

namespace Fcg.Users.Application.Features.Users.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }
        public TipoUser PerfilUser {  get; set; }
        public DateTime DataAlteracao { get; set; }
        public MotivoDesativacao? MotivoDesativacao { get;  set; }
    }
}
