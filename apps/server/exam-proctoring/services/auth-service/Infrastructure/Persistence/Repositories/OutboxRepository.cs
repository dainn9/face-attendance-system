// using auth_service.Application.Abstractions.Persistence;
// using auth_service.Domain.Outbox;
// using Microsoft.EntityFrameworkCore;

// namespace auth_service.Infrastructure.Persistence.Repositories
// {
//     public class OutboxRepository : IOutboxRepository
//     {
//         private readonly AuthDbContext _context;

//         public OutboxRepository(AuthDbContext context)
//         {
//             _context = context;
//         }

//         public void Add(OutboxMessage message, CancellationToken ct = default)
//         => _context.OutboxMessages.Add(message);

//         public Task<List<OutboxMessage>> GetPendingAsync(int limit, DateTime now, CancellationToken ct = default)
//         => _context.OutboxMessages
//             .Where(m => m.ProcessedAt == null && m.NextRetryAt <= now)
//             .OrderBy(m => m.OccurredAt)
//             .Take(limit)
//             .ToListAsync(ct);
//     }
// }
