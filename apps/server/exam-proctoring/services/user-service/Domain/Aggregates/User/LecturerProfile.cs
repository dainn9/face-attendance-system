using BuildingBlocks.Exceptions;

namespace user_service.Domain.Aggregates.User
{
    public class LecturerProfile
    {
        public Guid UserId { get; private set; }
        public Guid FacultyId { get; private set; }

        private LecturerProfile() { }

        public static LecturerProfile Create(Guid userId, Guid facultyId)
        {
            if (userId == Guid.Empty)
                throw new BusinessRuleViolationException("User ID cannot be empty.", ErrorCodes.InvalidLecturerProfile);

            if (facultyId == Guid.Empty)
                throw new BusinessRuleViolationException("Faculty ID cannot be empty.", ErrorCodes.InvalidLecturerProfile);

            return new LecturerProfile
            {
                UserId = userId,
                FacultyId = facultyId
            };
        }
    }
}