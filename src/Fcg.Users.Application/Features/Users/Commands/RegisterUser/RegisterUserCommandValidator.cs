using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Users.Application.Features.Users.Commands.RegisterUser
{
    public class CadastrarUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public CadastrarUserCommandValidator()
        {
            
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(DomainMessages.UserNameRequired)
                .Length(3, 50).WithMessage(DomainMessages.UserNameLengthInvalid);

            
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(DomainMessages.UserEmailRequired)
                .EmailAddress().WithMessage(DomainMessages.EmailInvalid)
                .Length(7, 100).WithMessage(DomainMessages.EmailLengthInvalid)
                .Must(email => !email.Contains("..")).WithMessage(DomainMessages.EmailInvalid)
                .Must(email=> email.Contains(".")).WithMessage(DomainMessages.EmailInvalid);

            
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(DomainMessages.UserPasswordRequired)
                .MinimumLength(8).WithMessage(DomainMessages.PasswordLengthInvalid)
                .MaximumLength(60).WithMessage(DomainMessages.PasswordLengthInvalid)
                .Matches(@"[A-Z]+").WithMessage("Sua senha deve conter pelo menos uma letra maiúscula.")
                .Matches(@"[a-z]+").WithMessage("Sua senha deve conter pelo menos uma letra minúscula.")
                .Matches(@"[0-9]+").WithMessage("Sua senha deve conter pelo menos um número.")
                .Matches(@"[\!\?\*\.\@]+").WithMessage("Sua senha deve conter pelo menos um caractere especial (!? *.@).");

            
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(DomainMessages.UserPasswordConfirmationRequired)
                .Equal(x => x.Password).WithMessage(DomainMessages.UserPasswordConfirmationMismatch);
        }
    }
}
