using BuildingBlocks.Abstractions.Persistence;

namespace attendance_service.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AttendanceDbContext _context;

        public UnitOfWork(AttendanceDbContext Context)
        {
            _context = Context;
        }
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);

    }
}