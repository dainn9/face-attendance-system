using BuildingBlocks.Exceptions;

namespace attendance_service.Domain.Aggregates.CourseSection
{
    public class Enrollment
    {
        public Guid Id { get; private set; }
        public Guid CourseSectionId { get; private set; }
        public Guid StudentId { get; private set; }
        public DateTime EnrolledAt { get; private set; }

        private Enrollment() { }

        public static Enrollment Create(Guid courseSectionId, Guid studentId, DateTime now)
        {
            if (courseSectionId == Guid.Empty)
                throw new BusinessRuleViolationException("Course section ID cannot be empty.", ErrorCodes.InvalidEnrollmentData);

            if (studentId == Guid.Empty)
                throw new BusinessRuleViolationException("Student ID cannot be empty.", ErrorCodes.InvalidEnrollmentData);

            return new Enrollment
            {
                Id = Guid.NewGuid(),
                CourseSectionId = courseSectionId,
                StudentId = studentId,
                EnrolledAt = now
            };
        }
    }
}