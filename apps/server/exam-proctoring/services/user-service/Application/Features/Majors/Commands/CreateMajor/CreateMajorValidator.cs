using BuildingBlocks.Validation;
using FluentValidation;

namespace user_service.Application.Features.Majors.Commands.CreateMajor
{
    public class CreateMajorValidator : AbstractValidator<CreateMajorCommand>
    {
        public CreateMajorValidator()
        {
            RuleFor(x => x.FacultyId)
            .ValidGuid();

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .MaxLength(100);

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .MaximumLength(20);
        }
    }
}