
using BuildingBlocks.Abstractions.Persistence;

namespace auth_service.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AuthDbContext _context;

        public UnitOfWork(AuthDbContext dbContext)
        {
            _context = dbContext;
        }
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
    }
}