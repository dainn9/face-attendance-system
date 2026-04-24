
using BuildingBlocks.Validation;
using FluentValidation;

namespace auth_service.Application.Features.Auth.Commands.LoginProctor
{
    public class LoginProctorValidator : AbstractValidator<LoginProctorCommand>
    {
        public LoginProctorValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .EmailAddress()
                .WithMessage(ValidationMessages.InvalidEmail);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .MinimumLength(8)
                .WithMessage(ValidationMessages.MinLength);
        }
    }
}