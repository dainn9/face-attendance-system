namespace user_service.Application.Contracts.Lecturers
{
    public record LecturerDto(
       Guid UserId,
       string UserCode,
       string FullName
   );
}