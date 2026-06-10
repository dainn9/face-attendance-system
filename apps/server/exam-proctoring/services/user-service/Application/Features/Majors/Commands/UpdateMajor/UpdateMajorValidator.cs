using BuildingBlocks.Validation;
using FluentValidation;

namespace user_service.Application.Features.Majors.Commands.UpdateMajor
{
    public class UpdateMajorValidator : AbstractValidator<UpdateMajorCommand>
    {
        public UpdateMajorValidator()
        {
            RuleFor(x => x.MajorId)
                .ValidGuid();

            RuleFor(x => x.FacultyId)
                .ValidGuid();

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("Code is required.")
                .MaximumLength(20);
        }
    }
}