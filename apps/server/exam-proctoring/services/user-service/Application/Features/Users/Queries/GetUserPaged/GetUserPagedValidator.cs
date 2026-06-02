using BuildingBlocks.Validation;
using FluentValidation;

namespace user_service.Application.Features.Users.Queries.GetUserPaged
{
    public class GetUserPagedValidator : AbstractValidator<GetUserPagedQuery>
    {
        public GetUserPagedValidator()
        {
            RuleFor(x => x.Page)
               .GreaterThan(0)
               .WithMessage(ValidationMessages.PageMustBeGreaterThanZero);

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("PageSize must be between 1 and 100.");
        }
    }
}