using BuildingBlocks.Validation;
using FluentValidation;

namespace auth_service.Application.Features.Auth.Commands.LoginProfile
{
    public class LoginProfileValidation : AbstractValidator<LoginProfileCommand>
    {
        public LoginProfileValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .EmailAddress()
                .WithMessage(ValidationMessages.InvalidEmail);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required);
        }
    }
}