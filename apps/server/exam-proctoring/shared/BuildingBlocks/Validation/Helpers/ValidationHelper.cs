using System.Text.RegularExpressions;

namespace BuildingBlocks.Validation.Helpers
{
    public static class ValidationHelper
    {
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        private static readonly Regex PasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$", RegexOptions.Compiled);

        private static readonly Regex OtpRegex = new(@"^\d{6}$", RegexOptions.Compiled);

        public static bool IsValidEmail(string email) => EmailRegex.IsMatch(email) && email.Length <= 255;

        public static bool IsStrongPassword(string password) => PasswordRegex.IsMatch(password);

        public static bool IsValidOtp(string otp) => OtpRegex.IsMatch(otp);

        public static bool IsValidMaxLength(string value, int maxLength) => value.Length <= maxLength;
    }
}