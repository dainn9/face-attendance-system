using BuildingBlocks.Exceptions;
using SharedKernel.Core;

namespace attendance_service.Domain.Aggregates.CourseSection
{
    public class CourseSession : AggregateRoot<Guid>
    {
        public Guid CourseId { get; private set; }
        public string SectionCode { get; private set; } = null!;
        public string Semester { get; private set; } = null!;
        public string AcademicYear { get; private set; } = null!;
        public Guid LecturerId { get; private set; }

        private List<Enrollment> _enrollments = new();
        public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

        private CourseSession() { }

        public static CourseSession Create(Guid courseId, string sectionCode, string semester, string academicYear, Guid lecturerId, DateTime now)
        {
            if (courseId == Guid.Empty)
                throw new BusinessRuleViolationException("Course ID cannot be empty.", ErrorCodes.InvalidCourseSectionData);

            if (string.IsNullOrWhiteSpace(sectionCode))
                throw new BusinessRuleViolationException("Section code cannot be empty.", ErrorCodes.InvalidCourseSectionData);

            if (string.IsNullOrWhiteSpace(semester))
                throw new BusinessRuleViolationException("Semester cannot be empty.", ErrorCodes.InvalidCourseSectionData);

            if (string.IsNullOrWhiteSpace(academicYear))
                throw new BusinessRuleViolationException("Academic year cannot be empty.", ErrorCodes.InvalidCourseSectionData);

            if (lecturerId == Guid.Empty)
                throw new BusinessRuleViolationException("Lecturer ID cannot be empty.", ErrorCodes.InvalidCourseSectionData);

            var courseSession = new CourseSession
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                SectionCode = sectionCode,
                Semester = semester,
                AcademicYear = academicYear,
                LecturerId = lecturerId
            };

            courseSession.SetCreated(now);
            courseSession.SetUpdated(now);

            return courseSession;
        }

        public void AddEnrollment(Guid studentId, DateTime now)
        {
            if (_enrollments.Any(e => e.StudentId == studentId))
                throw new BusinessRuleViolationException("Student already enrolled.", ErrorCodes.StudentAlreadyEnrolled);

            var enrollment = Enrollment.Create(Id, studentId, now);
            _enrollments.Add(enrollment);
            SetUpdated(now);
        }

        public void RemoveEnrollment(Guid studentId, DateTime now)
        {
            var enrollment = _enrollments.FirstOrDefault(e => e.StudentId == studentId)
                ?? throw new BusinessRuleViolationException("Student not enrolled.", ErrorCodes.StudentNotEnrolled);

            _enrollments.Remove(enrollment);
            SetUpdated(now);
        }

        public void AddEnrollments(IEnumerable<Guid> studentIds, DateTime now)
        {
            foreach (var studentId in studentIds)
            {
                if (_enrollments.Any(e => e.StudentId == studentId))
                    throw new BusinessRuleViolationException($"Student with ID {studentId} already enrolled.", ErrorCodes.StudentAlreadyEnrolled);

                var enrollment = Enrollment.Create(Id, studentId, now);
                _enrollments.Add(enrollment);
            }

            SetUpdated(now);
        }
    }
}