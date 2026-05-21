using BuildingBlocks.Validation;
using FluentValidation;

namespace user_service.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .ValidGuid();

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .MaxLength(100);

            RuleFor(x => x.Gender)
                .IsInEnum()
                .WithMessage(ValidationMessages.InvalidGender);

            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required);

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .EmailAddress()
                .WithMessage(ValidationMessages.InvalidEmail)
                .MaxLength(500);
        }
    }
}