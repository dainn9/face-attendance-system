using Microsoft.EntityFrameworkCore;
using user_service.Application.Abstractions.Persistence;
using user_service.Application.Contracts;
using user_service.Domain.Aggregates.User;

namespace user_service.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext Context) => _context = Context;

        public void Add(User user)
        => _context.Users.Add(user);

        public Task<UserDto?> GetProfileByIdAsync(Guid userId, CancellationToken cancellationToken)
        => _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserDto(
                u.Id,
                u.FullName,
                u.Gender,
                u.DateOfBirth,
                u.Email,
                u.Role,

                u.StudentProfile != null ? u.StudentProfile.StudentCode : null,

                u.StudentProfile != null ? u.StudentProfile.ClassCode : null,

                u.LecturerProfile != null ? u.LecturerProfile.FacultyCode : null
            ))
            .FirstOrDefaultAsync(cancellationToken);

        public Task<bool> ExistsByIdAsync(Guid userId, CancellationToken cancellationToken)
        => _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId, cancellationToken);

        public Task<bool> ExistsByStudentCodeAsync(string studentCode, CancellationToken cancellationToken)
        => _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.StudentProfile != null && u.StudentProfile.StudentCode == studentCode, cancellationToken);
    }
}