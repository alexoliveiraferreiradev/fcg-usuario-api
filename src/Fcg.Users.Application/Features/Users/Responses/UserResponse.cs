using Fcg.Users.Domain.Enum;

namespace Fcg.Users.Application.Features.Users.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public UserRole PerfilUser {  get; set; }
        public DateTime UpdatedAt { get; set; }
        public DeactivationReason? DeactivationReason { get;  set; }
    }
}
