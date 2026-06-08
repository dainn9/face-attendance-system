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

        public Task<bool> ExistsByCodeAsync(string code, Guid? excludedId = null, CancellationToken cancellationToken = default)
        => _context.CourseSections.AnyAsync(cs => cs.CourseSectionCode == code && (!excludedId.HasValue || cs.Id != excludedId.Value), cancellationToken);

        public async Task<ScheduleConflictDto?> GetRoomScheduleConflictAsync(
            IReadOnlyList<ScheduleDto> schedules,
            Semester semester,
            string academicYear,
            Guid LecturerId,
            Guid? excludeCourseSectionId = null,
            CancellationToken cancellationToken = default)
        {
            foreach (var newS in schedules)
            {
                var conflict = await _context.CourseSections
                    .AsNoTracking()
                    .Where(cs => (!excludeCourseSectionId.HasValue || cs.Id != excludeCourseSectionId.Value)
                        && cs.Semester == semester
                        && cs.AcademicYear == academicYear)
                    .SelectMany(cs => cs.Schedules, (cs, s) => new { cs, s })
                    .Where(x =>
                        x.s.Room == newS.Room &&
                        x.s.DayOfWeek == newS.DayOfWeek &&
                        x.s.StartTime < newS.EndTime &&
                        x.s.EndTime > newS.StartTime
                    )
                    .Select(x => new ScheduleConflictDto(
                        x.cs.CourseSectionCode,
                        x.s.DayOfWeek,
                        x.s.StartTime,
                        x.s.EndTime,
                        x.s.Room,
                        ScheduleConflictReason.Room
                    ))
                    .FirstOrDefaultAsync(cancellationToken);

                if (conflict != null)
                    return conflict;

                var lecturerConflict = await _context.CourseSections
                    .AsNoTracking()
                    .Where(cs => (!excludeCourseSectionId.HasValue || cs.Id != excludeCourseSectionId.Value)
                        && cs.Semester == semester
                        && cs.AcademicYear == academicYear
                        && cs.LecturerId == LecturerId)
                    .SelectMany(cs => cs.Schedules, (cs, s) => new { cs, s })
                    .Where(x =>
                        x.s.DayOfWeek == newS.DayOfWeek &&
                        x.s.StartTime < newS.EndTime &&
                        x.s.EndTime > newS.StartTime
                    )
                    .Select(x => new ScheduleConflictDto(
                        x.cs.CourseSectionCode,
                        x.s.DayOfWeek,
                        x.s.StartTime,
                        x.s.EndTime,
                        x.s.Room,
                        ScheduleConflictReason.Lecturer
                    ))
                    .FirstOrDefaultAsync(cancellationToken);

                if (lecturerConflict != null)
                    return lecturerConflict;
            }

            return null;
        }

        public Task<CourseSectionDetailDto?> GetCourseSectionDetailAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.CourseSections
                .AsNoTracking()
                .Where(cs => cs.Id == id)
                .Join(
                    _context.Subjects.AsNoTracking(),
                    cs => cs.SubjectId,
                    s => s.Id,
                    (cs, s) => new CourseSectionDetailDto(
                        cs.Id,
                        s.Name,
                        s.Credits,
                        cs.CourseSectionCode,
                        cs.LecturerId,
                        cs.IsActive,
                        cs.Semester,
                        cs.AcademicYear,
                        cs.MaxCapacity,
                        cs.Enrollments.Count,
                        cs.Schedules
                            .OrderBy(s => s.DayOfWeek)
                            .ThenBy(s => s.StartTime)
                            .Select(s => new ScheduleDetailDto(
                                s.Id,
                                s.DayOfWeek,
                                s.StartTime,
                                s.EndTime,
                                s.Room
                            ))
                            .ToList()
                    )
                )
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<PagedResult<Guid>> GetEnrolledStudentIdsPagedAsync(
            Guid courseSectionId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default
        )
        {
            var query = _context.CourseSections
                .AsNoTracking()
                .Where(cs => cs.Id == courseSectionId)
                .SelectMany(cs => cs.Enrollments)
                .OrderBy(e => e.EnrolledAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => e.StudentId)
                .ToListAsync(cancellationToken);

            return new PagedResult<Guid>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedResult<LecturerCourseSectionDto>> GetLecturerCourseSectionsAsync(
            Guid lecturerId,
            int page,
            int pageSize,
            string? searchQuery = null,
            Semester? semester = null,
            string? academicYear = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default
        )
        {
            var query = _context.CourseSections
                .AsNoTracking()
                .Where(cs => cs.LecturerId == lecturerId)
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
                        IsActive = cs.IsActive,
                        Semester = cs.Semester,
                        AcademicYear = cs.AcademicYear,
                        EnrollmentCount = cs.Enrollments.Count,
                        Schedules = cs.Schedules
                            .OrderBy(s => s.DayOfWeek)
                            .ThenBy(s => s.StartTime)
                            .Select(s => new ScheduleDto(
                                s.DayOfWeek,
                                s.StartTime,
                                s.EndTime,
                                s.Room
                            ))
                            .ToList()
                    }
                );

            if (!string.IsNullOrWhiteSpace(searchQuery))
                query = query.Where(c => c.SubjectName.Contains(searchQuery));

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
                .Select(c => new LecturerCourseSectionDto(
                    c.Id,
                    c.SubjectName,
                    c.SubjectCode,
                    c.CourseSectionCode,
                    c.IsActive,
                    c.Semester,
                    c.AcademicYear,
                    c.EnrollmentCount,
                    c.Schedules
                ))
                .ToListAsync();

            return new PagedResult<LecturerCourseSectionDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IReadOnlyList<LecturerCourseSectionLookupDto>> GetLecturerCourseSectionLookupAsync(
            Guid lecturerId,
            CancellationToken cancellationToken = default
        )
        => await _context.CourseSections
            .AsNoTracking()
            .Where(cs => cs.LecturerId == lecturerId)
            .Select(cs => new
            {
                cs.Semester,
                cs.AcademicYear
            })
            .Distinct()
            .OrderBy(x => x.AcademicYear)
            .ThenBy(x => x.Semester)
            .Select(x => new LecturerCourseSectionLookupDto(
                x.Semester,
                x.AcademicYear,
                $"{x.Semester} - {x.AcademicYear}"
            ))
            .ToListAsync(cancellationToken);

        public Task<bool> IsStudentEnrolledAsync(Guid courseSectionId, Guid userId, CancellationToken cancellationToken = default)
        => _context.CourseSections
            .AnyAsync(cs => cs.Id == courseSectionId && cs.Enrollments.Any(e => e.StudentId == userId), cancellationToken);
    }
}