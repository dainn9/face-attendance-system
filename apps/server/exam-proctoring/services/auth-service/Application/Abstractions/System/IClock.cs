namespace auth_service.Application.Abstractions.System
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}