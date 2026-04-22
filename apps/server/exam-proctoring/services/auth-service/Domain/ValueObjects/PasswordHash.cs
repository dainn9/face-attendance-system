using BuildingBlocks.Exceptions;

namespace auth_service.Domain.ValueObjects
{
    public class PasswordHash
    {
        public string Value { get; private set; }

        public PasswordHash(string value)
        {
            Value = value;
        }

        public static PasswordHash Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleViolationException("Password hash cannot be empty", ErrorCodes.PasswordEmpty);

            return new PasswordHash(value);
        }
    }
}