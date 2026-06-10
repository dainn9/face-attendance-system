// using auth_service.Domain.Outbox;

// namespace auth_service.Application.Abstractions.Persistence
// {
//     public interface IOutboxRepository
//     {
//         void Add(OutboxMessage message, CancellationToken ct = default);
//         Task<List<OutboxMessage>> GetPendingAsync(int limit, DateTime now, CancellationToken ct = default);
//     }
// }