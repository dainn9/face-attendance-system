using user_service.Application.Contracts.Majors;

namespace user_service.Application.Contracts.Faculties
{
    public record FacultyDto(
        Guid Id,
        string Name,
        string Code,
        int MajorCount,
        int StudentCount,
        int LecturerCount,
        MajorDto[] Majors
    );
}