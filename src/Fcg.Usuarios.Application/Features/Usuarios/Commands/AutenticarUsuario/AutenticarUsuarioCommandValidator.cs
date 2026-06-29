using FluentValidation;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.AutenticarUsuario
{
    public class AutenticarUsuarioCommandValidator : AbstractValidator<AutenticarUsuarioCommand>
    {
        public AutenticarUsuarioCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioEmailObrigatorio)
                .EmailAddress().WithMessage(MensagensDominio.EmailInvalido);

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioSenhaObrigatoria);
        }
    }
}
