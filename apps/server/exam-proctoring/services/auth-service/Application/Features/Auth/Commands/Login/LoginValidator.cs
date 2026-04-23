using BuildingBlocks.Validation;
using FluentValidation;

namespace auth_service.Application.Features.Auth.Commands.Login
{
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
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