using user_service.Application.Contracts.Lecturers;
using user_service.Application.Contracts.Majors;

namespace user_service.Application.Contracts.Faculties
{
    public record FacultyDetailDto(
        Guid Id,
        string Name,
        string Code,
        int MajorCount,
        int StudentCount,
        int LecturerCount,
        MajorDto[] Majors,
        LecturerDto[] Lecturers
    );
}