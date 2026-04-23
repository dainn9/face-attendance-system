namespace BuildingBlocks.Exceptions
{
    public class ErrorCodes
    {
        // ======= GENERAL =======
        public const string NotFound = "NOT_FOUND";
        public const string EntityNotFound = "ENTITY_NOT_FOUND";
        public const string Unauthorized = "UNAUTHORIZED";
        public const string InvalidRefreshToken = "INVALID_REFRESH_TOKEN";
        public const string ConfigurationError = "CONFIGURATION_ERROR";

        // ======= ACCOUNT =======
        public const string AccountNotFound = "ACCOUNT_NOT_FOUND";
        public const string AccountDisabled = "ACCOUNT_DISABLED";
        public const string AccountLocked = "ACCOUNT_LOCKED";

        // ======= EMAIL =======
        public const string EmailEmpty = "EMAIL_EMPTY";
        public const string InvalidEmail = "INVALID_EMAIL";

        // ======= PASSWORD =======
        public const string PasswordEmpty = "PASSWORD_EMPTY";

        // ======= REFRESH TOKEN =======
        public const string RefreshTokenStoreFailed = "REFRESH_TOKEN_STORE_FAILED";

    }
}