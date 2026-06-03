using BuildingBlocks.Exceptions;
using SharedKernel.Core;

namespace attendance_service.Domain.Aggregates.Subject
{
    public class Subject : AggregateRoot<Guid>
    {
        public string Name { get; private set; } = null!;
        public string Code { get; private set; } = null!;
        public int Credits { get; private set; }

        private Subject() { }

        public static Subject Create(string name, string code, int credits, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Name is required.", ErrorCodes.InvalidSubjectData);

            if (string.IsNullOrWhiteSpace(code))
                throw new BusinessRuleViolationException("Code is required.", ErrorCodes.InvalidSubjectData);

            if (credits <= 0)
                throw new BusinessRuleViolationException("Credits must be greater than 0.", ErrorCodes.InvalidSubjectData);

            var subject = new Subject
            {
                Id = Guid.NewGuid(),
                Name = name,
                Code = code,
                Credits = credits,
            };

            subject.SetCreated(now);
            subject.SetUpdated(now);

            return subject;
        }

        public void Update(string name, string code, int credits, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessRuleViolationException("Name is required.", ErrorCodes.InvalidSubjectData);

            if (string.IsNullOrWhiteSpace(code))
                throw new BusinessRuleViolationException("Code is required.", ErrorCodes.InvalidSubjectData);

            if (credits <= 0)
                throw new BusinessRuleViolationException("Credits must be greater than 0.", ErrorCodes.InvalidSubjectData);

            Name = name;
            Code = code;
            Credits = credits;

            SetUpdated(now);
        }
    }
}