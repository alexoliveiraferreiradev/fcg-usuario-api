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
                .Length(7, 100).WithMessage(DomainMessages.EmailLengthInvalid);

            
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(DomainMessages.UserPasswordRequired)
                .MinimumLength(8).WithMessage(DomainMessages.PasswordLengthInvalid)
                .MaximumLength(60).WithMessage(DomainMessages.PasswordLengthInvalid);

            
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(DomainMessages.UserPasswordConfirmationRequired)
                .Equal(x => x.Password).WithMessage(DomainMessages.UserPasswordConfirmationMismatch);
        }
    }
}
