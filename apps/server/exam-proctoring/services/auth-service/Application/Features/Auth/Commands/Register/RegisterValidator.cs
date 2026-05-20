using BuildingBlocks.Validation;
using FluentValidation;

namespace auth_service.Application.Features.Auth.Commands.Register
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .EmailAddress()
                .WithMessage(ValidationMessages.InvalidEmail);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .PasswordComplexity();

            RuleFor(x => x.UserRole)
                .IsInEnum()
                .WithMessage(ValidationMessages.InvalidUserRole);
        }
    }
}