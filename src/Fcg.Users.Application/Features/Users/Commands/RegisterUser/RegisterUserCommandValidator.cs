using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Users.Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(DomainMessages.UserNameRequired)
                .Length(3, 50).WithMessage(DomainMessages.UserNameLengthInvalid)
                .NotEqual("nome do usuário").WithMessage(DomainMessages.NameNotReal);

            
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
                .Matches(@"[A-Z]+").WithMessage(DomainMessages.PasswordMustContainUppercase)
                .Matches(@"[a-z]+").WithMessage(DomainMessages.PasswordMustContainLowercase)
                .Matches(@"[0-9]+").WithMessage(DomainMessages.PasswordMustContainNumber)
                .Matches(@"[\!\?\*\.\@]+").WithMessage(DomainMessages.PasswordMustContainSpecialCharacter);

            
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(DomainMessages.UserPasswordConfirmationRequired)
                .Equal(x => x.Password).WithMessage(DomainMessages.UserPasswordConfirmationMismatch);
        }
    }
}
