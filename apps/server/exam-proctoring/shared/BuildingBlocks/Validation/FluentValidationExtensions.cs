using BuildingBlocks.Validation.Helpers;
using FluentValidation;

namespace BuildingBlocks.Validation
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, Guid> ValidGuid<T>(this IRuleBuilder<T, Guid> ruleBuilder)
        {
            return ruleBuilder
                .NotEqual(Guid.Empty)
                .WithMessage(ValidationMessages.InvalidGuid);
        }

        public static IRuleBuilderOptions<T, string> PasswordComplexity<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .MinimumLength(8)
                .WithMessage(ValidationMessages.MinLength)
                .Must(ValidationHelper.IsStrongPassword)
                .WithMessage(ValidationMessages.PasswordComplexity);
        }

        public static IRuleBuilderOptions<T, string> MaxLength<T>(this IRuleBuilder<T, string> ruleBuilder, int maxLength)
        {
            return ruleBuilder
                .MaximumLength(maxLength)
                .WithMessage(ValidationMessages.MaxLength);
        }
    }
}