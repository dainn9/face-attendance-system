using BuildingBlocks.Validation;
using FluentValidation;

namespace attendance_service.Application.Features.Attendances.Queries.GetStudentCourseSectionAttendanceRecords
{
    public class GetStudentCourseSectionAttendanceRecordsValidator : AbstractValidator<GetStudentCourseSectionAttendanceRecordsQuery>
    {
        public GetStudentCourseSectionAttendanceRecordsValidator()
        {
            RuleFor(x => x.StudentId)
                .ValidGuid();

            RuleFor(x => x.CourseSectionId)
                .ValidGuid();
        }
    }
}