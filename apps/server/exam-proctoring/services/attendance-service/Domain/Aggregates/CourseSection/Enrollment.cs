namespace attendance_service.Domain.Aggregates.CourseSection
{
    public class Enrollment
    {
        public Guid Id { get; private set; }
        public Guid CourseSessionId { get; private set; }
        public Guid StudentId { get; private set; }
        public DateTime EnrolledAt { get; private set; }

        private Enrollment() { }

        public static Enrollment Create(Guid courseSessionId, Guid studentId, DateTime now)
        {
            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                CourseSessionId = courseSessionId,
                StudentId = studentId,
                EnrolledAt = now
            };

            return enrollment;
        }
    }
}