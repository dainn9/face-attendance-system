using BuildingBlocks.Exceptions;

namespace user_service.Domain.Aggregates.User
{
    public class StudentProfile
    {
        public Guid UserId { get; private set; }
        public string StudentCode { get; private set; } = null!;
        public string FacultyCode { get; private set; } = null!;
        public string MajorCode { get; private set; } = null!;

        private StudentProfile() { }

        public static StudentProfile Create(Guid userId, string studentCode, string facultyCode, string majorCode)
        {
            if (string.IsNullOrWhiteSpace(studentCode))
                throw new BusinessRuleViolationException("Student code cannot be empty.", ErrorCodes.InvalidStudentProfile);

            if (string.IsNullOrWhiteSpace(facultyCode))
                throw new BusinessRuleViolationException("Faculty code cannot be empty.", ErrorCodes.InvalidStudentProfile);

            if (string.IsNullOrWhiteSpace(majorCode))
                throw new BusinessRuleViolationException("Major code cannot be empty.", ErrorCodes.InvalidStudentProfile);

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