using FluentValidation;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Users.Application.Features.Users.Commands.UpdateUser
{
    public class AtualizarUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public AtualizarUserCommandValidator()
        {
            
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(DomainMessages.UserNameRequired)
                .Length(3, 50).WithMessage(DomainMessages.UserNameLengthInvalid);

            
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
