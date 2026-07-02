using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Users.Application.Features.Users.Commands.CadastrarUser
{
    public class CadastrarUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public CadastrarUserCommandValidator()
        {
            
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioNomeObrigatorio)
                .Length(3, 50).WithMessage(MensagensDominio.UsuarioTamanhoNomeInvalido);

            
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioEmailObrigatorio)
                .EmailAddress().WithMessage(MensagensDominio.EmailInvalido)
                .Length(7, 100).WithMessage(MensagensDominio.EmailTamanhoInvalido);

            
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioSenhaObrigatoria)
                .MinimumLength(8).WithMessage(MensagensDominio.SenhaTamanhoInvalido)
                .MaximumLength(60).WithMessage(MensagensDominio.SenhaTamanhoInvalido);

            
            RuleFor(x => x.ConfirmacaoSenha)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioConfirmacaoSenhaObrigatoria)
                .Equal(x => x.Password).WithMessage(MensagensDominio.UsuarioSenhaConfirmacaoDiferente);
        }
    }
}
