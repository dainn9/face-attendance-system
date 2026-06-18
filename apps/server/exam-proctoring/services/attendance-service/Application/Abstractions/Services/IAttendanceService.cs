namespace attendance_service.Application.Abstractions.Services
{
    public interface IAttendanceService
    {
        public Task CloseExpiredSessionsAsync(CancellationToken cancellationToken = default);
    }
}