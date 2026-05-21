using SharedKernel.Core;
using SharedKernel.Core.Enums;

namespace user_service.Domain.Aggregates.User
{
    public class User : AggregateRoot<Guid>
    {
        public string FullName { get; private set; } = null!;
        public Gender Gender { get; private set; }
        public DateOnly DateOfBirth { get; private set; }
        public string Email { get; private set; } = null!;

        private User() { }

        public static User Create(Guid userId, string fullName, Gender gender, DateOnly dateOfBirth, string email, DateTime now)
        {
            var user = new User
            {
                Id = userId,
                FullName = fullName,
                Gender = gender,
                DateOfBirth = dateOfBirth,
                Email = email
            };

            user.SetCreated(now);
            user.SetUpdated(now);

            return user;
        }
    }
}