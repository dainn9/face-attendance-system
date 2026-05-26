using BuildingBlocks.Exceptions;

namespace user_service.Domain.Aggregates.User
{
    public class LecturerProfile
    {
        public Guid UserId { get; private set; }
        public string LecturerCode { get; private set; } = null!;
        public string FacultyCode { get; private set; } = null!;

        private LecturerProfile() { }

        public static LecturerProfile Create(Guid userId, string lecturerCode, string facultyCode)
        {
            if (userId == Guid.Empty)
                throw new BusinessRuleViolationException("User ID cannot be empty.", ErrorCodes.InvalidLecturerProfile);

            if (string.IsNullOrWhiteSpace(lecturerCode))
                throw new BusinessRuleViolationException("Lecturer code cannot be empty.", ErrorCodes.InvalidLecturerProfile);

            if (string.IsNullOrWhiteSpace(facultyCode))
                throw new BusinessRuleViolationException("Faculty code cannot be empty.", ErrorCodes.InvalidLecturerProfile);

            return new LecturerProfile
            {
                UserId = userId,
                LecturerCode = lecturerCode,
                FacultyCode = facultyCode
            };
        }
    }
}