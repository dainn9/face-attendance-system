namespace BuildingBlocks.Validation
{
    public class ValidationMessages
    {
        public const string Required = "{PropertyName} is required.";
        public const string InvalidEmail = "Please enter a valid email address.";
        public const string MinLength = "This field must be at least {MinLength} characters long.";
        public const string MaxLength = "This field cannot exceed {MaxLength} characters.";
        // public const string Range = "Please enter a value between {0} and {1}.";
        // public const string InvalidFormat = "The format of this field is invalid.";
    }
}