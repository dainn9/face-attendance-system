using BuildingBlocks.Validation;
using FluentValidation;

namespace attendance_service.Application.Features.Courses.Queries.GetById
{
    public class GetByIdValidator : AbstractValidator<GetByIdQuery>
    {
        public GetByIdValidator()
        {
            RuleFor(x => x.CourseId)
                .ValidGuid();
        }
    }
}