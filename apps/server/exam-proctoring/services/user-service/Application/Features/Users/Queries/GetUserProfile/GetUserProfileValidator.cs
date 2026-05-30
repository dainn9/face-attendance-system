// using BuildingBlocks.Validation;
// using FluentValidation;

// namespace user_service.Application.Features.Users.Queries.GetUserProfile
// {
//     public class GetUserProfileValidator : AbstractValidator<GetUserProfileQuery>
//     {
//         public GetUserProfileValidator()
//         {
//             RuleFor(x => x.UserId)
//                 .NotEmpty()
//                 .WithMessage(ValidationMessages.Required)
//                 .ValidGuid();
//         }
//     }
// }