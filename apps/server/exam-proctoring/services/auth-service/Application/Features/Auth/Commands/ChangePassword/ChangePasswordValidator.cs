using BuildingBlocks.Validation;
using FluentValidation;

namespace auth_service.Application.Features.Auth.Commands.ChangePassword
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .ValidGuid();

            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required);

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .PasswordComplexity();

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .Equal(x => x.NewPassword)
                .WithMessage("Passwords do not match.");
        }
    }
}