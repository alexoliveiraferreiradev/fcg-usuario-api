using FluentValidation;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Application.Features.Users.Commands.AtualizarUser
{
    public class AtualizarUserCommandValidator : AbstractValidator<AtualizarUserCommand>
    {
        public AtualizarUserCommandValidator()
        {
            
            RuleFor(x => x.NomeUser)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioNomeObrigatorio)
                .Length(3, 50).WithMessage(MensagensDominio.UsuarioTamanhoNomeInvalido);

            
            RuleFor(x => x.SenhaUser)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioSenhaObrigatoria)
                .MinimumLength(8).WithMessage(MensagensDominio.SenhaTamanhoInvalido)
                .MaximumLength(60).WithMessage(MensagensDominio.SenhaTamanhoInvalido);

            
            RuleFor(x => x.ConfirmacaoSenha)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioConfirmacaoSenhaObrigatoria)
                .Equal(x => x.SenhaUser).WithMessage(MensagensDominio.UsuarioSenhaConfirmacaoDiferente);
        }
    }
}
