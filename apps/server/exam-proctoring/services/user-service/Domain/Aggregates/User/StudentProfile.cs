using BuildingBlocks.Exceptions;

namespace user_service.Domain.Aggregates.User
{
    public class StudentProfile
    {
        public Guid UserId { get; private set; }
        public Guid MajorId { get; private set; }

        private StudentProfile() { }

        public static StudentProfile Create(Guid userId, Guid majorId)
        {
            if (userId == Guid.Empty)
                throw new BusinessRuleViolationException("User ID cannot be empty.", ErrorCodes.InvalidStudentProfile);

            if (majorId == Guid.Empty)
                throw new BusinessRuleViolationException("Major ID cannot be empty.", ErrorCodes.InvalidStudentProfile);

            return new StudentProfile
            {
                UserId = userId,
                MajorId = majorId
            };
        }
    }
}