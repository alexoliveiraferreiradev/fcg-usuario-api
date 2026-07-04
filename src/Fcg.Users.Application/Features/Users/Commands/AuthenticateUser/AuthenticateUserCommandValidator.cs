using FluentValidation;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Application.Features.Users.Commands.AuthenticateUser
{
    public class AuthenticateUserCommandValidator : AbstractValidator<AuthenticateUserCommand>
    {
        public AuthenticateUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(DomainMessages.UserEmailRequired)
                .EmailAddress().WithMessage(DomainMessages.EmailInvalid);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(DomainMessages.UserPasswordRequired);
        }
    }
}
