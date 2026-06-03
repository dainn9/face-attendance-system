using BuildingBlocks.Validation;
using FluentValidation;

namespace attendance_service.Application.Features.Subjects.Commands.UpdateSubject
{
    public class UpdateSubjectValidator : AbstractValidator<UpdateSubjectCommand>
    {
        public UpdateSubjectValidator()
        {
            RuleFor(x => x.SubjectId)
                .ValidGuid();

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .MaxLength(100);

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .MaxLength(20);

            RuleFor(x => x.Credits)
                .GreaterThan(0)
                .WithMessage(ValidationMessages.GreaterThanZero);
        }
    }
}