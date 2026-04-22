using FluentValidation;

namespace BuildingBlocks.Validation
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, Guid> ValidGuid<T>(this IRuleBuilder<T, Guid> ruleBuilder)
        {
            return ruleBuilder
                .NotEqual(Guid.Empty)
                .WithMessage("Guid không được rỗng");
        }
    }
}