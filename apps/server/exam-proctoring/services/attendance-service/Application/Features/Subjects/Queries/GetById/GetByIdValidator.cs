using BuildingBlocks.Validation;
using FluentValidation;

namespace attendance_service.Application.Features.Subjects.Queries.GetById
{
    public class GetByIdValidator : AbstractValidator<GetByIdQuery>
    {
        public GetByIdValidator()
        {
            RuleFor(x => x.SubjectId)
                .ValidGuid();
        }
    }
}