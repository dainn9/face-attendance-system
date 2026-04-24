using BuildingBlocks.Validation;
using FluentValidation;

namespace auth_service.Application.Features.Auth.Commands.LoginAdmin
{
    public class LoginAdminValidator : AbstractValidator<LoginAdminCommand>
    {
        public LoginAdminValidator()
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