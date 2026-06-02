using BuildingBlocks.Validation;
using FluentValidation;

namespace user_service.Application.Features.Majors.Queries.GetMajorLookupByFacultyId
{
    public class GetMajorLookupByFacultyIdValidator : AbstractValidator<GetMajorLookupByFacultyIdQuery>
    {
        public GetMajorLookupByFacultyIdValidator()
        {
            RuleFor(x => x.FacultyId)
                .ValidGuid();
        }
    }
}