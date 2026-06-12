using attendance_service.Application.Contracts;
using attendance_service.Domain.Enums;
using BuildingBlocks.Results;

namespace attendance_service.Application.Abstractions.Persistence
{
    public interface ICourseSectionReadRepository
    {
        Task<PagedResult<CourseSectionPagedDto>> GetCourseSectionsPagedAsync(
            int page = 1,
            int pageSize = 12,
            string? searchQuery = null,
            Guid? facultyId = null,
            Semester? semester = null,
            string? academicYear = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default
        );

        Task<bool> ExistsByCodeAsync(string code, Guid? excludedId = null, CancellationToken cancellationToken = default);

        Task<ScheduleConflictDto?> GetRoomScheduleConflictAsync(
            IReadOnlyList<ScheduleDto> schedules,
            Semester semester,
            string academicYear,
            Guid lecturerId,
            Guid? excludeCourseSectionId = null,
            CancellationToken cancellationToken = default);

        Task<CourseSectionDetailDto?> GetCourseSectionDetailAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<Guid>> GetEnrolledStudentIdsPagedAsync(
            Guid courseSectionId,
            int page = 1,
            int pageSize = 12,
            CancellationToken cancellationToken = default
        );

        Task<PagedResult<LecturerCourseSectionDto>> GetLecturerCourseSectionsAsync(
            Guid lecturerId,
            int page = 1,
            int pageSize = 12,
            string? searchQuery = null,
            Semester? semester = null,
            string? academicYear = null,
            bool? isActive = null,
            CancellationToken cancellationToken = default
        );

        Task<IReadOnlyList<LecturerCourseSectionLookupDto>> GetLecturerCourseSectionLookupAsync(
            Guid lecturerId,
            CancellationToken cancellationToken = default
        );

        Task<bool> IsStudentEnrolledAsync(Guid courseSectionId, Guid userId, CancellationToken cancellationToken = default);
        Task<Guid?> GetLecturerIdAsync(Guid courseSectionId, CancellationToken cancellationToken);
        Task<HashSet<Guid>> GetEnrollmentStudentIdsAsync(Guid courseSectionId, CancellationToken cancellationToken);

        Task<IReadOnlyList<StudentCourseSectionDto>> GetStudentActiveCourseSectionsAsync(
            Guid studentId,
            CancellationToken cancellationToken = default
        );
    }
}