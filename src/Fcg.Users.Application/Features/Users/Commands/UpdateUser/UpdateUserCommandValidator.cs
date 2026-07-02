using FluentValidation;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Application.Features.Users.Commands.AtualizarUser
{
    public class AtualizarUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public AtualizarUserCommandValidator()
        {
            
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioNomeObrigatorio)
                .Length(3, 50).WithMessage(MensagensDominio.UsuarioTamanhoNomeInvalido);

            
            RuleFor(x => x.password)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioSenhaObrigatoria)
                .MinimumLength(8).WithMessage(MensagensDominio.SenhaTamanhoInvalido)
                .MaximumLength(60).WithMessage(MensagensDominio.SenhaTamanhoInvalido);

            
            RuleFor(x => x.ConfirmacaoSenha)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioConfirmacaoSenhaObrigatoria)
                .Equal(x => x.password).WithMessage(MensagensDominio.UsuarioSenhaConfirmacaoDiferente);
        }
    }
}
