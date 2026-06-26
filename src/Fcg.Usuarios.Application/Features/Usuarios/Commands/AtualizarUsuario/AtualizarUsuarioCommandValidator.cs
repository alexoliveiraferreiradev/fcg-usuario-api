using FluentValidation;
using Fcg.Usuarios.Domain.Constants;

namespace Fcg.Usuarios.Application.Features.Usuarios.Commands.AtualizarUsuario
{
    public class AtualizarUsuarioCommandValidator : AbstractValidator<AtualizarUsuarioCommand>
    {
        public AtualizarUsuarioCommandValidator()
        {
            
            RuleFor(x => x.NomeUsuario)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioNomeObrigatorio)
                .Length(3, 50).WithMessage(MensagensDominio.UsuarioTamanhoNomeInvalido);

            
            RuleFor(x => x.SenhaUsuario)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioSenhaObrigatoria)
                .MinimumLength(8).WithMessage(MensagensDominio.SenhaTamanhoInvalido)
                .MaximumLength(60).WithMessage(MensagensDominio.SenhaTamanhoInvalido);

            
            RuleFor(x => x.ConfirmacaoSenha)
                .NotEmpty().WithMessage(MensagensDominio.UsuarioConfirmacaoSenhaObrigatoria)
                .Equal(x => x.SenhaUsuario).WithMessage(MensagensDominio.UsuarioSenhaConfirmacaoDiferente);
        }
    }
}
