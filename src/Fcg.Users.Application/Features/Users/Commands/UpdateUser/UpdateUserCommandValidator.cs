using FluentValidation;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Application.Features.Users.Commands.AtualizarUser
{
    public class AtualizarUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public AtualizarUserCommandValidator()
        {
            
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(DomainMessages.UserNameRequired)
                .Length(3, 50).WithMessage(DomainMessages.UserNameLengthInvalid);

            
            RuleFor(x => x.password)
                .NotEmpty().WithMessage(DomainMessages.UserPasswordRequired)
                .MinimumLength(8).WithMessage(DomainMessages.PasswordLengthInvalid)
                .MaximumLength(60).WithMessage(DomainMessages.PasswordLengthInvalid);

            
            RuleFor(x => x.ConfirmacaoSenha)
                .NotEmpty().WithMessage(DomainMessages.UserPasswordConfirmationRequired)
                .Equal(x => x.password).WithMessage(DomainMessages.UserPasswordConfirmationMismatch);
        }
    }
}
