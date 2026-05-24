using auth_service.Domain.ValueObjects;
using BuildingBlocks.Exceptions;
using SharedKernel.Core;
using SharedKernel.Core.Enums;

namespace auth_service.Domain.Aggregates.User
{
    public class User : AggregateRoot<Guid>
    {
        public Email Email { get; private set; } = null!;
        public PasswordHash PasswordHash { get; private set; } = null!;
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; } = true;
        public int FailedLoginAttempts { get; private set; }
        public DateTime? LockoutEnd { get; private set; }

        private User() { }
        public static User Create(Email email, PasswordHash passwordHash, UserRole role, DateTime now)
        {
            if (email is null)
                throw new BusinessRuleViolationException("Email is required", ErrorCodes.EmailEmpty);

            if (passwordHash is null)
                throw new BusinessRuleViolationException("Password is required", ErrorCodes.PasswordEmpty);

            var user = new User
            {
                Id = Guid.NewGuid(),
                PasswordHash = passwordHash,
                Email = email,
                Role = role,
                IsActive = false,
            };

            user.SetCreated(now);
            user.SetUpdated(now);
            return user;
        }

        public void UpdateActiveStatus(bool status, DateTime now)
        {
            IsActive = status;
            SetUpdated(now);
        }

        public void ChangePassword(PasswordHash newPassword, DateTime now)
        {
            if (newPassword is null)
                throw new BusinessRuleViolationException("New password is required", ErrorCodes.PasswordEmpty);

            PasswordHash = newPassword;
            SetUpdated(now);
        }

        public void ChangeRole(UserRole newRole, DateTime now)
        {
            Role = newRole;
            SetUpdated(now);
        }

        public void RecordFailedLogin(int maxAttempts, TimeSpan lockoutDuration, DateTime now)
        {
            FailedLoginAttempts++;
            if (FailedLoginAttempts >= maxAttempts)
            {
                LockoutEnd = now.Add(lockoutDuration);
                FailedLoginAttempts = 0;
            }
            SetUpdated(now);
        }

        public void RecordSuccessfulLogin(DateTime now)
        {
            FailedLoginAttempts = 0;
            LockoutEnd = null;
            SetUpdated(now);
        }
        public bool IsLocked()
        {
            return LockoutEnd.HasValue && LockoutEnd > DateTime.UtcNow;
        }
    }
}