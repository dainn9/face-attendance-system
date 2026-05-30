namespace user_service.Application.Contracts
{
    public record FacultyListItemDto(
        FacultyDto Faculty,
        int StudentCount,
        int LecturerCount,
        MajorListItemDto[] Majors
    );

    public record FacultyDto(
        Guid Id,
        string Name,
        string Code,
        int MajorCount
    );

    public record MajorDto(
        Guid Id,
        string Name,
        string Code
    );

    public record MajorListItemDto(
        MajorDto Major,
        int StudentCount
    );
}