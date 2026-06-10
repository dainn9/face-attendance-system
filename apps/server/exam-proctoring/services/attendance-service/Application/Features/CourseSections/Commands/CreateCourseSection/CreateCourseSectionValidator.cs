using attendance_service.Domain.Enums;
using BuildingBlocks.Validation;
using FluentValidation;

namespace attendance_service.Application.Features.CourseSections.Commands.CreateCourseSection
{
    public class CreateCourseSectionValidator : AbstractValidator<CreateCourseSectionCommand>
    {
        public CreateCourseSectionValidator()
        {
            RuleFor(x => x.SubjectId)
                .ValidGuid();

            RuleFor(x => x.CourseSectionCode)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .MaxLength(20);

            RuleFor(x => x.Semester)
                .IsInEnum()
                .WithMessage("Invalid semester value.")
                .NotEqual(Semester.None)
                .WithMessage("Semester is required.");

            RuleFor(x => x.AcademicYear)
                .NotEmpty()
                .WithMessage(ValidationMessages.Required)
                .MaxLength(20)
                .Matches(@"^\d{4}-\d{4}$")
                .WithMessage("Academic year must have format YYYY-YYYY.");

            RuleFor(x => x.LecturerId)
                .ValidGuid();

            RuleFor(x => x.MaxCapacity)
                .GreaterThan(0)
                .WithMessage(ValidationMessages.GreaterThanZero);

            RuleFor(x => x.Schedules)
                .NotEmpty()
                .WithMessage("At least one schedule is required.");

            RuleForEach(x => x.Schedules).ChildRules(schedule =>
            {
                schedule.RuleFor(s => s.DayOfWeek)
                    .IsInEnum()
                    .WithMessage("Invalid day of week.");

                schedule.RuleFor(s => s.StartTime)
                    .LessThan(s => s.EndTime)
                    .WithMessage("Schedule start time must be before end time.");

                schedule.RuleFor(s => s.Room)
                    .NotEmpty()
                    .WithMessage(ValidationMessages.Required)
                    .MaxLength(50);
            });
        }
    }
}