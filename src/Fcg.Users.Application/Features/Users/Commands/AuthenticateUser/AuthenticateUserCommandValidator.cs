using FluentValidation;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Application.Features.Users.Commands.AutenticarUser
{
    public class AutenticarUserCommandValidator : AbstractValidator<AuthenticateUserCommand>
    {
        public AutenticarUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioEmailObrigatorio)
                .EmailAddress().WithMessage(MensagensDominio.EmailInvalido);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioSenhaObrigatoria);
        }
    }
}
