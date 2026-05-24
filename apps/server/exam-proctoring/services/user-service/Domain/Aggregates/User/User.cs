using SharedKernel.Core;
using SharedKernel.Core.Enums;
using BuildingBlocks.Exceptions;

namespace user_service.Domain.Aggregates.User
{
    public class User : AggregateRoot<Guid>
    {
        public string FullName { get; private set; } = null!;
        public Gender Gender { get; private set; }
        public DateOnly DateOfBirth { get; private set; }
        public string Email { get; private set; } = null!;
        public UserRole Role { get; private set; }

        public LecturerProfile? LecturerProfile { get; private set; }
        public StudentProfile? StudentProfile { get; private set; }

        private User() { }

        public static User Create(Guid userId, string fullName, Gender gender, DateOnly dateOfBirth, string email, UserRole role, DateTime now)
        {
            var user = new User
            {
                Id = userId,
                FullName = fullName,
                Gender = gender,
                DateOfBirth = dateOfBirth,
                Email = email,
                Role = role
            };

            user.SetCreated(now);
            user.SetUpdated(now);

            return user;
        }

        public void AddLecturerProfile(string lecturerCode, string facultyCode)
        {
            if (Role != UserRole.Lecturer)
                throw new BusinessRuleViolationException("Cannot add lecturer profile to a non-lecturer user.", ErrorCodes.InvalidUserRole);

            if (LecturerProfile is not null)
                throw new BusinessRuleViolationException("Lecturer profile already exists for this user.", ErrorCodes.LecturerProfileAlreadyExists);

            LecturerProfile = LecturerProfile.Create(
                Id,
                lecturerCode,
                facultyCode
            );
        }

        public void AddStudentProfile(string studentCode, string facultyCode, string majorCode)
        {
            if (Role != UserRole.Student)
                throw new BusinessRuleViolationException("Cannot add student profile to a non-student user.", ErrorCodes.InvalidUserRole);

            if (StudentProfile is not null)
                throw new BusinessRuleViolationException("Student profile already exists for this user.", ErrorCodes.StudentProfileAlreadyExists);

            StudentProfile = StudentProfile.Create(
                Id,
                studentCode,
                facultyCode,
                majorCode
            );
        }
    }
}