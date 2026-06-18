using attendance_service.Application.Abstractions.Services;
using BuildingBlocks.Time;

namespace attendance_service.Infrastructure.BackgroundJobs
{
    public class AttendanceSessionWorker : BackgroundService
    {
        private static readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
        private readonly ILogger<AttendanceSessionWorker> _logger;
        private readonly IClock _clock;
        private readonly IServiceScopeFactory _scopeFactory;

        public AttendanceSessionWorker(
            ILogger<AttendanceSessionWorker> logger,
            IClock clock,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _clock = clock;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_interval);

            while (!stoppingToken.IsCancellationRequested
                   && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await CheckAndCloseExpiredAttendanceSessionsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while closing expired attendance sessions.");
                }
            }
        }

        private async Task CheckAndCloseExpiredAttendanceSessionsAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Checking for expired attendance sessions at {Time}", _clock.UtcNow);

            using var scope = _scopeFactory.CreateScope();
            var attendanceService = scope.ServiceProvider.GetRequiredService<IAttendanceService>();

            await attendanceService.CloseExpiredSessionsAsync(cancellationToken);
        }
    }
}