using attendance_service.Application.Abstractions.Persistence;
using attendance_service.Application.Contracts;
using attendance_service.Domain.Enums;
using BuildingBlocks.Results;
using Microsoft.EntityFrameworkCore;

namespace attendance_service.Infrastructure.Persistence.Repositories
{
    public class CourseSectionReadRepository : ICourseSectionReadRepository
    {
        private readonly AttendanceDbContext _context;

        public CourseSectionReadRepository(AttendanceDbContext context) => _context = context;

        public async Task<PagedResult<CourseSectionPagedDto>> GetCourseSectionsPagedAsync(
            int page,
            int pageSize,
            string? searchQuery = null,
            Guid? facultyId = null,
            Semester? semester = null,
            string? academicYear = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.CourseSections
                .AsNoTracking()
                .Join(
                    _context.Subjects.AsNoTracking(),
                    cs => cs.SubjectId,
                    s => s.Id,
                    (cs, s) => new
                    {
                        Id = cs.Id,
                        SubjectName = s.Name,
                        SubjectCode = s.Code,
                        CourseSectionCode = cs.CourseSectionCode,
                        LecturerId = cs.LecturerId,
                        FacultyId = s.FacultyId,
                        IsActive = cs.IsActive,
                        Semester = cs.Semester,
                        AcademicYear = cs.AcademicYear,
                        EnrollmentCount = cs.Enrollments.Count,
                        FirstSchedule = cs.Schedules
                            .OrderBy(s => s.DayOfWeek)
                            .ThenBy(s => s.StartTime)
                            .Select(s => new ScheduleDto(
                                s.DayOfWeek,
                                s.StartTime,
                                s.EndTime,
                                s.Room
                            ))
                            .FirstOrDefault()
                    }
                );

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(c =>
                    c.SubjectName.Contains(searchQuery) ||
                    c.SubjectCode.Contains(searchQuery) ||
                    c.CourseSectionCode.Contains(searchQuery)
                );
            }

            if (facultyId.HasValue)
                query = query.Where(c => c.FacultyId == facultyId.Value);

            if (semester.HasValue)
                query = query.Where(c => c.Semester == semester.Value);

            if (!string.IsNullOrWhiteSpace(academicYear))
                query = query.Where(c => c.AcademicYear == academicYear);

            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(c => c.SubjectName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CourseSectionPagedDto(
                    c.Id,
                    c.SubjectName,
                    c.SubjectCode,
                    c.CourseSectionCode,
                    c.LecturerId,
                    c.FacultyId,
                    c.IsActive,
                    c.Semester,
                    c.AcademicYear,
                    c.EnrollmentCount,
                    c.FirstSchedule
                ))
                .ToListAsync();

            return new PagedResult<CourseSectionPagedDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}