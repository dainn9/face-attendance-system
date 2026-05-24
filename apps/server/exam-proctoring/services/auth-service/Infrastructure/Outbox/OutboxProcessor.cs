using auth_service.Application.Abstractions.Clients;
using auth_service.Application.Abstractions.Persistence;
using auth_service.Application.Contracts;
using auth_service.Application.IntegrationEvents;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Time;
using System.Text.Json;

namespace auth_service.Infrastructure.Outbox
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxProcessor> _logger;

        public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                var userClient = scope.ServiceProvider.GetRequiredService<IUserInternalClient>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var clock = scope.ServiceProvider.GetRequiredService<IClock>();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                var messages = await outboxRepository.GetPendingAsync(20, clock.UtcNow, stoppingToken);

                foreach (var message in messages)
                {
                    try
                    {
                        if (message.Type == nameof(UserRegisteredIntegrationEvent))
                        {
                            var payload = JsonSerializer.Deserialize<UserRegisteredIntegrationEvent>(message.Payload)!;

                            var authUser = await userRepository.GetTrackedByIdAsync(payload.UserId, stoppingToken);

                            if (authUser is null)
                            {
                                message.MarkFailed("Auth user not found", clock.UtcNow);
                                continue;
                            }

                            var createUserRequest = new CreateUserRequest(
                                payload.UserId,
                                payload.FullName,
                                payload.Gender,
                                payload.DateOfBirth,
                                payload.Email,
                                payload.Role,
                                payload.StudentCode,
                                payload.LecturerCode,
                                payload.FacultyCode,
                                payload.MajorCode
                            );

                            await userClient.CreateUserAsync(createUserRequest, stoppingToken);

                            authUser.UpdateActiveStatus(true, clock.UtcNow);
                        }

                        message.MarkProcessed(clock.UtcNow);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                        message.MarkFailed(ex.Message, clock.UtcNow);
                    }
                }

                await unitOfWork.SaveChangesAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
