using BuildingBlocks.Validation;
using FluentValidation;

namespace attendance_service.Application.Features.Attendances.Queries.GetAttendanceCheckInInfo
{
    public class GetAttendanceCheckInInfoValidator : AbstractValidator<GetAttendanceCheckInInfoQuery>
    {
        public GetAttendanceCheckInInfoValidator()
        {
            RuleFor(x => x.AttendanceSessionId)
                .ValidGuid();
        }
    }
}