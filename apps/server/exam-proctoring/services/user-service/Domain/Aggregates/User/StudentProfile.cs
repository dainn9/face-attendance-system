using BuildingBlocks.Exceptions;

namespace user_service.Domain.Aggregates.User
{
    public class StudentProfile
    {
        public Guid UserId { get; private set; }
        public string StudentCode { get; private set; } = null!;
        public string ClassCode { get; private set; } = null!;

        private StudentProfile() { }

        public static StudentProfile Create(Guid userId, string studentCode, string classCode)
        {
            if (userId == Guid.Empty)
                throw new BusinessRuleViolationException("User ID cannot be empty.", ErrorCodes.InvalidStudentProfile);

            if (string.IsNullOrWhiteSpace(studentCode))
                throw new BusinessRuleViolationException("Student code cannot be empty.", ErrorCodes.InvalidStudentProfile);

            if (string.IsNullOrWhiteSpace(classCode))
                throw new BusinessRuleViolationException("Class code cannot be empty.", ErrorCodes.InvalidStudentProfile);

            return new StudentProfile
            {
                UserId = userId,
                StudentCode = studentCode,
                ClassCode = classCode
            };
        }
    }
}