namespace BuildingBlocks.Validation
{
    public class ValidationMessages
    {
        public const string Required = "{PropertyName} is required.";
        public const string InvalidEmail = "Please enter a valid email address.";
        public const string MinLength = "This field must be at least {MinLength} characters long.";
        public const string MaxLength = "This field cannot exceed {MaxLength} characters.";
        public const string InvalidSessionType = "Invalid session type.";
        public const string PasswordComplexity = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";
        public const string InvalidGuid = "{PropertyName} cannot be empty.";
        public const string InvalidUserRole = "Invalid user role.";
        public const string InvalidGender = "Invalid gender.";
        public const string NotAllowed = "{PropertyName} is not allowed for this user role.";

        // public const string Range = "Please enter a value between {0} and {1}.";
        // public const string InvalidFormat = "The format of this field is invalid.";
    }
}