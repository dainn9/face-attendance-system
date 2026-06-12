using BuildingBlocks.Validation;
using FluentValidation;

namespace attendance_service.Application.Features.CourseSections.Queries.GetStudentActiveCourseSections
{
    public class GetStudentActiveCourseSectionsValidator : AbstractValidator<GetStudentActiveCourseSectionsQuery>
    {
        public GetStudentActiveCourseSectionsValidator()
        {
            RuleFor(x => x.StudentId)
                .ValidGuid();
        }
    }
}