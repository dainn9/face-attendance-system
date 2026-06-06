namespace user_service.Application.Contracts.Students
{
    public record StudentSummaryDto(
       Guid UserId,
       string UserCode,
       string FullName,
       string FacultyName
   );
}