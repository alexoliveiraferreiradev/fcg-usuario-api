using Fcg.Usuarios.Domain.Enum;
namespace Fcg.Usuarios.Application.Dtos
{
    public class DesativaUsuarioRequest
    {
        public Guid Id { get; set; }
        public MotivoDesativacao MotivoDelecao { get; set; }

        public DesativaUsuarioRequest()
        {            
        }

        public DesativaUsuarioRequest(Guid id, MotivoDesativacao motivoDelecao)
        {
            Id = id;
            MotivoDelecao = motivoDelecao;
        }
    }
        
}
