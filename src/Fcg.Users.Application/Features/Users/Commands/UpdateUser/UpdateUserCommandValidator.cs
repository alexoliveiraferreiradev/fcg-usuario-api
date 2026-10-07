using FluentValidation;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(DomainMessages.UserNameRequired)
                .Length(3, 50).WithMessage(DomainMessages.UserNameLengthInvalid);

            
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
