using BuildingBlocks.Validation;
using FluentValidation;

namespace attendance_service.Application.Features.Attendances.Commands.CreateAttendanceSession
{
    public class CreateAttendanceSessionValidator : AbstractValidator<CreateAttendanceSessionCommand>
    {
        public CreateAttendanceSessionValidator()
        {
            RuleFor(x => x.CourseSectionId)
                .ValidGuid();
        }
    }
}