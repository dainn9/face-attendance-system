using BuildingBlocks.Validation;
using FluentValidation;
using SharedKernel.Core.Enums;

namespace user_service.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.UserId)
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

            RuleFor(x => x.Role)
               .IsInEnum()
               .WithMessage(ValidationMessages.InvalidUserRole);

            When(x => x.Role == UserRole.Student, () =>
            {
                RuleFor(x => x.MajorId)
                    .NotNull()
                    .WithMessage(ValidationMessages.InvalidGuid);

                RuleFor(x => x.FacultyId)
                    .Empty()
                    .WithMessage(ValidationMessages.NotAllowed);

                RuleFor(x => x.UserCode)
                   .NotEmpty()
                   .WithMessage(ValidationMessages.Required)
                   .MaximumLength(20);
            });

            When(x => x.Role == UserRole.Lecturer, () =>
            {
                RuleFor(x => x.FacultyId)
                    .NotNull()
                    .WithMessage(ValidationMessages.InvalidGuid);

                RuleFor(x => x.MajorId)
                    .Empty()
                    .WithMessage(ValidationMessages.NotAllowed);

                RuleFor(x => x.UserCode)
                   .NotEmpty()
                   .WithMessage(ValidationMessages.Required)
                   .MaximumLength(20);
            });

            When(x => x.Role == UserRole.Admin, () =>
            {
                RuleFor(x => x.FacultyId)
                    .Empty()
                    .WithMessage(ValidationMessages.NotAllowed);

                RuleFor(x => x.MajorId)
                    .Empty()
                    .WithMessage(ValidationMessages.NotAllowed);

                RuleFor(x => x.UserCode).Empty();
            });
        }
    }
}