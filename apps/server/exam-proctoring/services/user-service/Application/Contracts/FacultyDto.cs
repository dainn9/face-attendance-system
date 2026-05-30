namespace user_service.Application.Contracts
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

    public record MajorDto(
        Guid Id,
        string Name,
        string Code,
        int StudentCount
    );

    public record MajorListItemDto(
        MajorDto Major,
        int StudentCount
    );
}