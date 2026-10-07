using System.ComponentModel;

namespace Fcg.User.API.Dto
{
    public record UpdateUserRequest(
    [property: DefaultValue("novo nome do usuário")] string Name,
    [property: DefaultValue("nova senha do usuário")] string Password,
    [property: DefaultValue("confirmação da nova senha do usuário")] string ConfirmPassword
    );
}
