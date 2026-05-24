using BuildingBlocks.Abstractions.Persistence;

namespace user_service.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserDbContext _context;

        public UnitOfWork(UserDbContext dbContext)
        {
            _context = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
    }
}