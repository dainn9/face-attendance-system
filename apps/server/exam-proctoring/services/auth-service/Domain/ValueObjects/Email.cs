using BuildingBlocks.Exceptions;
using BuildingBlocks.Validation.Helpers;

namespace auth_service.Domain.ValueObjects
{
    public class Email
    {
        public string Value { get; }
        private Email(string value)
        {
            Value = value;
        }
        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleViolationException("Email cannot be empty", ErrorCodes.EmailEmpty);

            if (!ValidationHelper.IsValidEmail(value))
                throw new BusinessRuleViolationException("Email is not valid or exceeds maximum length", ErrorCodes.InvalidEmail);

            return new Email(value.ToLower());
        }

        // public override bool Equals(object? obj)
        // {
        //     return obj is Email other && Value == other.Value;
        // }

        // public override int GetHashCode()
        // {
        //     return Value.GetHashCode();
        // }
    }
}