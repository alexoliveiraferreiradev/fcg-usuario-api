using FluentValidation;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Application.Features.Users.Commands.AutenticarUser
{
    public class AutenticarUserCommandValidator : AbstractValidator<AutenticarUserCommand>
    {
        public AutenticarUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioEmailObrigatorio)
                .EmailAddress().WithMessage(MensagensDominio.EmailInvalido);

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioSenhaObrigatoria);
        }
    }
}
