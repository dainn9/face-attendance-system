using BuildingBlocks.Validation;
using FluentValidation;

namespace auth_service.Application.Features.Auth.Commands.CreateAccount
{
    public class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
    {
        public CreateAccountValidator()
        {
            // RuleFor(x => x.FullName)
            //     .NotEmpty()
            //     .WithMessage(ValidationMessages.Required)
            //     .MaxLength(100);

            // RuleFor(x => x.Gender)
            //     .IsInEnum()
            //     .WithMessage(ValidationMessages.InvalidGender);

            // RuleFor(x => x.DateOfBirth)
            //     .NotEmpty()
            //     .WithMessage(ValidationMessages.Required);

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

            // When(x => x.UserRole == UserRole.Student, () =>
            // {
            //     RuleFor(x => x.StudentCode)
            //         .NotEmpty()
            //         .WithMessage(ValidationMessages.Required)
            //         .MaximumLength(20);

            //     RuleFor(x => x.ClassCode)
            //         .NotEmpty()
            //         .WithMessage(ValidationMessages.Required)
            //         .MaximumLength(20);

            //     RuleFor(x => x.FacultyCode)
            //        .Empty()
            //        .WithMessage(ValidationMessages.NotAllowed);
            // });

            // When(x => x.UserRole == UserRole.Lecturer, () =>
            // {
            //     RuleFor(x => x.FacultyCode)
            //         .NotEmpty()
            //         .WithMessage(ValidationMessages.Required)
            //         .MaximumLength(20);

            //     RuleFor(x => x.StudentCode)
            //         .Empty()
            //         .WithMessage(ValidationMessages.NotAllowed);

            //     RuleFor(x => x.ClassCode)
            //         .Empty()
            //         .WithMessage(ValidationMessages.NotAllowed);
            // });

            // When(x => x.UserRole == UserRole.Admin, () =>
            // {
            //     RuleFor(x => x.StudentCode)
            //         .Empty()
            //         .WithMessage(ValidationMessages.NotAllowed);

            //     RuleFor(x => x.FacultyCode)
            //         .Empty()
            //         .WithMessage(ValidationMessages.NotAllowed);

            //     RuleFor(x => x.ClassCode)
            //         .Empty()
            //         .WithMessage(ValidationMessages.NotAllowed);
            // });
        }
    }
}