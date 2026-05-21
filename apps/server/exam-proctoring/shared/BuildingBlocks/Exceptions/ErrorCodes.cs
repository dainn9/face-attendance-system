namespace BuildingBlocks.Exceptions
{
    public class ErrorCodes
    {
        // ======= GENERAL =======
        public const string Token_Expired = "TOKEN_EXPIRED";
        public const string NotFound = "NOT_FOUND";
        public const string EntityNotFound = "ENTITY_NOT_FOUND";
        public const string Unauthorized = "UNAUTHORIZED";
        public const string InvalidSessionType = "INVALID_SESSION_TYPE";
        public const string InvalidRefreshToken = "INVALID_REFRESH_TOKEN";
        public const string ConfigurationError = "CONFIGURATION_ERROR";

        // ======= ACCOUNT =======
        public const string AccountNotFound = "ACCOUNT_NOT_FOUND";
        public const string AccountNotActive = "ACCOUNT_NOT_ACTIVE";
        public const string AccountLocked = "ACCOUNT_LOCKED";

        // ======= EMAIL =======
        public const string EmailEmpty = "EMAIL_EMPTY";
        public const string InvalidEmail = "INVALID_EMAIL";
        public const string EmailAlreadyExists = "EMAIL_ALREADY_EXISTS";

        // ======= PASSWORD =======
        public const string PasswordEmpty = "PASSWORD_EMPTY";
        public const string PasswordIncorrect = "PASSWORD_INCORRECT";

        // ======= REFRESH TOKEN =======
        public const string RefreshTokenStoreFailed = "REFRESH_TOKEN_STORE_FAILED";

    }
}