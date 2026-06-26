using FluentValidation;
using Fcg.Usuarios.Domain.Constants;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.CadastrarUsuario
{
    public class CadastrarUsuarioCommandValidator : AbstractValidator<CadastrarUsuarioCommand>
    {
        public CadastrarUsuarioCommandValidator()
        {
            
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioNomeObrigatorio)
                .Length(3, 50).WithMessage(MensagensDominio.UsuarioTamanhoNomeInvalido);

            
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioEmailObrigatorio)
                .EmailAddress().WithMessage(MensagensDominio.EmailInvalido)
                .Length(7, 100).WithMessage(MensagensDominio.EmailTamanhoInvalido);

            
            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioSenhaObrigatoria)
                .MinimumLength(8).WithMessage(MensagensDominio.SenhaTamanhoInvalido)
                .MaximumLength(60).WithMessage(MensagensDominio.SenhaTamanhoInvalido);

            
            RuleFor(x => x.ConfirmacaoSenha)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioConfirmacaoSenhaObrigatoria)
                .Equal(x => x.Senha).WithMessage(MensagensDominio.UsuarioSenhaConfirmacaoDiferente);
        }
    }
}
