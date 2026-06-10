namespace auth_service.API.Contracts
{
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword, string ConfirmPassword);
}