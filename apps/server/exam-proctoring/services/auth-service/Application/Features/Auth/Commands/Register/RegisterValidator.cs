using BuildingBlocks.Validation;
using FluentValidation;
using SharedKernel.Core.Enums;

namespace auth_service.Application.Features.Auth.Commands.Register
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator()
        {
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
                .WithMessage(ValidationMessages.InvalidEmail);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .PasswordComplexity();

            RuleFor(x => x.UserRole)
                .IsInEnum()
                .WithMessage(ValidationMessages.InvalidUserRole);

            When(x => x.UserRole == UserRole.Student, () =>
            {
                RuleFor(x => x.StudentCode)
                    .NotEmpty()
                    .WithMessage(ValidationMessages.Required)
                    .MaximumLength(20);

                RuleFor(x => x.FacultyCode)
                    .NotEmpty()
                    .WithMessage(ValidationMessages.Required)
                    .MaximumLength(20);

                RuleFor(x => x.MajorCode)
                    .NotEmpty()
                    .WithMessage(ValidationMessages.Required)
                    .MaximumLength(20);

                RuleFor(x => x.LecturerCode)
                    .Empty()
                    .WithMessage(ValidationMessages.NotAllowed);
            });

            When(x => x.UserRole == UserRole.Lecturer, () =>
            {
                RuleFor(x => x.LecturerCode)
                    .NotEmpty()
                    .WithMessage(ValidationMessages.Required)
                    .MaximumLength(20);

                RuleFor(x => x.FacultyCode)
                    .NotEmpty()
                    .WithMessage(ValidationMessages.Required)
                    .MaximumLength(20);

                RuleFor(x => x.StudentCode)
                    .Empty()
                    .WithMessage(ValidationMessages.NotAllowed);

                RuleFor(x => x.MajorCode)
                    .Empty()
                    .WithMessage(ValidationMessages.NotAllowed);
            });

            When(x => x.UserRole == UserRole.Admin, () =>
            {
                RuleFor(x => x.StudentCode)
                    .Empty()
                    .WithMessage(ValidationMessages.NotAllowed);

                RuleFor(x => x.FacultyCode)
                    .Empty()
                    .WithMessage(ValidationMessages.NotAllowed);

                RuleFor(x => x.MajorCode)
                    .Empty()
                    .WithMessage(ValidationMessages.NotAllowed);

                RuleFor(x => x.LecturerCode)
                    .Empty()
                    .WithMessage(ValidationMessages.NotAllowed);
            });
        }
    }
}