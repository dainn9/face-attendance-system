using BuildingBlocks.Validation;
using FluentValidation;

namespace attendance_service.Application.Features.Courses.Queries.GetPaged
{
    public class GetPagedValidator : AbstractValidator<GetPagedQuery>
    {
        public GetPagedValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage(ValidationMessages.PageMustBeGreaterThanZero);
        }
    }
}