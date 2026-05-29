using BuildingBlocks.Validation;
using FluentValidation;

namespace user_service.Application.Features.Faculties.Commands.CreateFaculty
{
    public class CreateFacultyValidator : AbstractValidator<CreateFacultyCommand>
    {
        public CreateFacultyValidator()
        {
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