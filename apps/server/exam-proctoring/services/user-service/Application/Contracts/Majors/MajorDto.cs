namespace user_service.Application.Contracts.Majors
{
    public record MajorDto(
        Guid Id,
        string Name,
        string Code,
        int StudentCount
    );
}