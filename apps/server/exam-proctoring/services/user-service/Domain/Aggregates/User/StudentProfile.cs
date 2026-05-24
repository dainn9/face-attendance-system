namespace user_service.Domain.Aggregates.User
{
    public class StudentProfile
    {
        public Guid UserId { get; private set; }
        public string StudentCode { get; private set; } = null!;

        public string FacultyCode { get; private set; } = null!;

        public string MajorCode { get; private set; } = null!;

        public User User { get; private set; } = null!;

        private StudentProfile() { }

        public static StudentProfile Create(Guid userId, string studentCode, string facultyCode, string majorCode)
        {
            return new StudentProfile
            {
                UserId = userId,
                StudentCode = studentCode,
                FacultyCode = facultyCode,
                MajorCode = majorCode
            };
        }
    }
}