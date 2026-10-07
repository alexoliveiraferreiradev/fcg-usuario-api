using Fcg.Users.Domain.Enum;

namespace Fcg.User.API.Dto
{
    public record UpdateUserResponse(
        string NewName,
        string Email
    );
}
