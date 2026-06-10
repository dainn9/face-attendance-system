using BuildingBlocks.Validation;
using FluentValidation;

namespace attendance_service.Application.Features.Attendances.Commands.CheckInAttendance
{
    public class CheckInAttendanceValidator : AbstractValidator<CheckInAttendanceCommand>
    {
        public CheckInAttendanceValidator()
        {
            RuleFor(x => x.AttendanceSessionId)
                .ValidGuid();

            RuleFor(x => x.StudentId)
                .ValidGuid();
            RuleFor(x => x.Confidence)
                .InclusiveBetween(0, 1)
                .WithMessage("Confidence must be between 0 and 1.");
        }
    }
}